using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using TodoAppELK.Controllers;
using TodoAppELK.Models.Domain;
using TodoAppELK.Models.DTOs;
using TodoAppELK.Services.Abstract;

namespace TodoAppELK.Tests;

public class TodoControllerTests
{
    private readonly Mock<ITodoService> _serviceMock;
    private readonly TodoController _controller;

    public TodoControllerTests()
    {
        _serviceMock = new Mock<ITodoService>();
        _controller = new TodoController(_serviceMock.Object);

        // Controller'ýn içindeki User.FindFirst'in çalýþmasý için sahte bir kullanýcý oluþturuyoruz.
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1") // Kullanýcý ID'si 1 olarak ayarlandý
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkResult_WhenTodosExist()
    {
        // Arrange
        var todos = new List<TodoResponseDto>
        {
            new TodoResponseDto { Id = 1, Title = "Test Todo 1", Description = "Test Todo Desc" },
            new TodoResponseDto { Id = 2, Title = "Test Todo 2", Description = "Test Todo Desc 2" }
        };

        _serviceMock.Setup(s => s.GetAllByUserIdAsync(1)).ReturnsAsync(todos);
        
        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedTodos = Assert.IsAssignableFrom<List<TodoResponseDto>>(okResult.Value);
        Assert.Equal(2, returnedTodos.Count);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenTodoDoesNotExist()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetByIdAsync(1, 1)).ReturnsAsync((TodoResponseDto?)null);

        // Act ( mock kullanýcý ID'si 1 olarak ayarlandý)
        var result = await _controller.GetById(1);  

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Add_ShouldReturnCreatedTodo_WhenTodoIsValid()
    {
        // Arrange
        var newTodo = new AddTodoDto { Title = "New Todo", Description = "New Todo Desc", IsCompleted = false };

        var createdTodo = new TodoResponseDto { Id = 1, Title = "New Todo", Description = "New Todo Desc", IsCompleted = false };
        
        _serviceMock.Setup(s => s.AddAsync(newTodo, 1)).ReturnsAsync(createdTodo);

        // Act
        var result = await _controller.Add(newTodo);

        // Assert
        // 1. Dönen sonucun türünün 'CreatedAtActionResult' (HTTP 201) olduðunu doðrula.
        result.Should().BeOfType<CreatedAtActionResult>();

        // 2. Sonucu doðru türe dönüþtür.
        var createdAtActionResult = result as CreatedAtActionResult;
        createdAtActionResult.Should().NotBeNull();

        // 3. Yanýtýn gövdesindeki verinin, servisin döndürmesi gereken DTO ile ayný olduðunu doðrula.
        createdAtActionResult.Value.Should().BeEquivalentTo(createdTodo);

        // 4. Location header'ýnýn doðru oluþturulduðunu doðrula.
        // Yani, sonucun doðru 'GetById' action'ýna ve doðru ID'ye iþaret ettiðinden emin ol.
        createdAtActionResult.ActionName.Should().Be(nameof(TodoController.GetById));
        createdAtActionResult.RouteValues["id"].Should().Be(1);
    }


}

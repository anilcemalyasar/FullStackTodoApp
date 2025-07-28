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

        // Controller'�n i�indeki User.FindFirst'in �al��mas� i�in sahte bir kullan�c� olu�turuyoruz.
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1") // Kullan�c� ID'si 1 olarak ayarland�
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

        // Act ( mock kullan�c� ID'si 1 olarak ayarland�)
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
        // 1. D�nen sonucun t�r�n�n 'CreatedAtActionResult' (HTTP 201) oldu�unu do�rula.
        result.Should().BeOfType<CreatedAtActionResult>();

        // 2. Sonucu do�ru t�re d�n��t�r.
        var createdAtActionResult = result as CreatedAtActionResult;
        createdAtActionResult.Should().NotBeNull();

        // 3. Yan�t�n g�vdesindeki verinin, servisin d�nd�rmesi gereken DTO ile ayn� oldu�unu do�rula.
        createdAtActionResult.Value.Should().BeEquivalentTo(createdTodo);

        // 4. Location header'�n�n do�ru olu�turuldu�unu do�rula.
        // Yani, sonucun do�ru 'GetById' action'�na ve do�ru ID'ye i�aret etti�inden emin ol.
        createdAtActionResult.ActionName.Should().Be(nameof(TodoController.GetById));
        createdAtActionResult.RouteValues["id"].Should().Be(1);
    }


}

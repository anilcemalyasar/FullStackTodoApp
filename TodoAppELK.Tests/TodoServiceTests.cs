using FluentAssertions;
using Moq;
using TodoAppELK.Models.Domain;
using TodoAppELK.Models.DTOs;
using TodoAppELK.Repositories.Abstract;
using TodoAppELK.Services.Concrete;

namespace TodoAppELK.Tests;

public class TodoServiceTests
{
    private readonly Mock<ITodoRepository> _repositoryMock;
    private readonly TodoService _service;

    public TodoServiceTests()
    {
        _repositoryMock = new Mock<ITodoRepository>();
        _service = new TodoService(_repositoryMock.Object);

    }

    [Fact]
    public async Task GetAllByUserIdAsync_ShouldReturnMappedTodos_WhenTodosExists()
    {
        // Arrange
        var userId = 1;
        var todos = new List<Todo>
        {
            new Todo { Id = 1, Title = "Test Todo 1", UserId = userId },
            new Todo { Id = 2, Title = "Test Todo 2", UserId = userId }
        };

        // Mock the repository to return the test todos
        _repositoryMock.Setup(r => r.GetAllByUserIdAsync(userId)).ReturnsAsync(todos);

        // Act
        var result = await _service.GetAllByUserIdAsync(userId);


        // Assert
        Assert.NotNull(result);
        result.Should().HaveCount(2);
        result.Should().BeOfType<List<TodoResponseDto>>(); 
        Assert.Equal("Test Todo 1", result[0].Title);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMappedTodo_WhenTodoExists()
    {
        // Arrange
        var userId = 1;
        var todo = new Todo
        {
            Id = 1, Title = "Test Todo", Description = "Test Description", UserId = userId
        };

        // Mock the repository to return the test todo
        _repositoryMock.Setup(r => r.GetByIdAsync(todo.Id, userId)).ReturnsAsync(todo);

        // Act
        var result = await _service.GetByIdAsync(todo.Id, userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<TodoResponseDto>();
        result.Id.Should().Be(todo.Id);
        result.Title.Should().Be("Test Todo");
    }

    [Fact]
    public async Task AddAsync_ShouldReturnMappedTodo_WhenTodoIsAdded()
    {
        // Arrange
        var userId = 1;
        var addDto = new AddTodoDto
        {
            Title = "New Todo",
            Description = "New Description",
            IsCompleted = false
        };

        var todoToAdd = new Todo
        {
            Title = addDto.Title,
            Description = addDto.Description,
            IsCompleted = addDto.IsCompleted,
            UserId = userId,
        };

        // Mock the repository to return the added todo
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Todo>())).ReturnsAsync(todoToAdd);

        // Act
        var result = await _service.AddAsync(addDto, userId);
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<TodoResponseDto>();
        result.Title.Should().Be("New Todo");
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenDeletionIsSuccessful()
    {
        // Arrange
        var userId = 1;
        var todoId = 1;
        // Repository'nin true döneceðini varsay
        _repositoryMock.Setup(r => r.DeleteAsync(todoId, userId)).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAsync(todoId, userId);

        // Assert
        result.Should().BeTrue();
    }


}

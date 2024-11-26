using TaskManager.Controllers;
using TaskManager.Data;
using TaskManager.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace TaskManager.Tests;

public class UnitTest1
{
    private readonly ApplicationDbContext _context;

    public UnitTest1()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TaskManagerTest")
            .Options;
        _context = new ApplicationDbContext(options);

        // Reinicia la base de datos antes de cada prueba
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        // Seed data para pruebas
        _context.Tasks.AddRange(
            new TaskItem { Name = "Task 1", Description = "Test Task 1" },
            new TaskItem { Name = "Task 2", Description = "Test Task 2" }
        );
        _context.SaveChanges();
    }

    [Fact]
    public void Test_GetAllTasks()
    {
        // Arrange
        var controller = new TasksController(_context);

        // Act
        var result = controller.Index();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, _context.Tasks.Count());
    }

    [Fact]
    public void Test_CreateTask()
    {
        // Arrange
        var controller = new TasksController(_context);
        var newTask = new TaskItem { Name = "Task 3", Description = "Test Task 3" };

        // Act
        controller.Create(newTask);
        _context.SaveChanges();

        // Assert
        Assert.Equal(3, _context.Tasks.Count());
        Assert.Contains(_context.Tasks, t => t.Name == "Task 3");
    }

    [Fact]
    public void Test_DeleteTask()
    {
        // Arrange
        var controller = new TasksController(_context);
        var taskToDelete = _context.Tasks.First();

        // Act
        controller.DeleteConfirmed(taskToDelete.Id);
        _context.SaveChanges();

        // Assert
        Assert.Equal(1, _context.Tasks.Count());
        Assert.DoesNotContain(_context.Tasks, t => t.Id == taskToDelete.Id);
    }
}
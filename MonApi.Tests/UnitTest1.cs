using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonApi.Controllers;
using MonApi.Data;
using MonApi.Models;

namespace MonApi.Tests
{
    public class TasksControllerTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetAll_ReturneListeTaches()
        {
            // Arrange
            var context = GetDbContext();
            context.Tasks.Add(new TaskItem { Id = 1, Titre = "Test tâche", EstTerminée = false });
            await context.SaveChangesAsync();

            var controller = new TasksController(context);

            // Act
            var result = await controller.GetAll();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var liste = Assert.IsType<List<TaskItem>>(ok.Value);
            Assert.Single(liste);
        }

        [Fact]
        public async Task GetById_ReturneNotFound_SiInexistant()
        {
            // Arrange
            var context = GetDbContext();
            var controller = new TasksController(context);

            // Act
            var result = await controller.GetById(99);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_AjouteTache()
        {
            // Arrange
            var context = GetDbContext();
            var controller = new TasksController(context);
            var nouvelle = new TaskItem { Titre = "Nouvelle tâche", EstTerminée = false };

            // Act
            var result = await controller.Create(nouvelle);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(1, await context.Tasks.CountAsync());
        }

        [Fact]
        public async Task Delete_SupprimeTache()
        {
            // Arrange
            var context = GetDbContext();
            context.Tasks.Add(new TaskItem { Id = 1, Titre = "À supprimer", EstTerminée = false });
            await context.SaveChangesAsync();

            var controller = new TasksController(context);

            // Act
            var result = await controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Equal(0, await context.Tasks.CountAsync());
        }
    }
}
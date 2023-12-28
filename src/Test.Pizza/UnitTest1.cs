using Microsoft.AspNetCore.Identity;
using Moq;
using WebApi.Data.Models;
using WebApi.Data.Repositories;
using WebApi.Services.Pizza;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Test.Pizza
{
    public class UnitTest1
    {

        public static class MockUserManager
        {
            public static Mock<UserManager<User>> CreateMockUserManager(User user, string rol)
            {
                var store = new Mock<IUserStore<User>>();
                var userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
                userManagerMock.Setup(m => m.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
                userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new[] { rol }); //Retorno de Rol
                return userManagerMock;
            }
        }

        public class PizzaServiceTests
        {

            
            [Fact]
            public async Task GetPizzaAsync_ReturnsPizzaWhenCreatedByUser()
            {
                
                var user = new User {  Email ="userA@gmail.com", FullName="Usuario A", UserName="userA" };
                var userManagerMock = MockUserManager.CreateMockUserManager(user, "User");
                var pizzaRepositoryMock = new Mock<IPizzaRepository>();
                var pizzaService = new PizzaService(pizzaRepositoryMock.Object, userManagerMock.Object);

                var pizza = new WebApi.Data.Models.Pizza
                {
                    Id = 1,
                    CreatedBy = user.Id,
                    IsGlutenFree = true,
                    Name = "Peperoni",
                    Created = DateTime.UtcNow,
                    Price = 3,
                    Size = PizzaSize.Medium

                };
                // Simulacion que devuelve el metodod GetPizzaAsync
                pizzaRepositoryMock.Setup(repo => repo.GetPizzaAsync(pizza.Id)).ReturnsAsync(pizza);

                var pizza_solicitada = 1;
                var result = await pizzaService.GetPizzaAsync(pizza_solicitada, user.Id);

                Assert.Equal(pizza, result);
            }




            [Fact]
            public async Task AddPizzaAsync_ShouldAddPizza()
            {

                var user = new User { Email = "userB@gmail.com", FullName = "Usuario B", UserName = "userB" };
                var userManagerMock = MockUserManager.CreateMockUserManager(user, "User");

                var pizzaRepositoryMock = new Mock<IPizzaRepository>();
                var pizzaService = new PizzaService(pizzaRepositoryMock.Object, userManagerMock.Object);

                var newPizza = new WebApi.Data.Models.Pizza
                {
                    Id = 2,
                    CreatedBy = user.Id,
                    IsGlutenFree = false,
                    Name = "Hawayana",
                    Created = DateTime.UtcNow,
                    Price = 2,
                    Size = PizzaSize.Small
                };

                pizzaRepositoryMock.Setup(repo => repo.AddPizzaAsync(It.IsAny<WebApi.Data.Models.Pizza>())).ReturnsAsync(newPizza);
                var addedPizza = await pizzaService.AddPizzaAsync(newPizza, user.Id);

                Assert.Equal(newPizza, addedPizza);
                
            }

            [Fact]
            public async Task AddPizzaAsync_ShouldAddPizzaForAdmin()
            {

                var user = new User { Email = "admin_1gmail.com", FullName = "Admin A", UserName = "adminA" };
                var userManagerMock = MockUserManager.CreateMockUserManager(user, "Admin");

                var pizzaRepositoryMock = new Mock<IPizzaRepository>();
                var pizzaService = new PizzaService(pizzaRepositoryMock.Object, userManagerMock.Object);

                var newPizza = new WebApi.Data.Models.Pizza
                {
                    Id = 6,
                    CreatedBy = user.Id,
                    IsGlutenFree = false,
                    Name = "Hawayana medium",
                    Created = DateTime.UtcNow,
                    Price = 3,
                    Size = PizzaSize.Medium
                };

                pizzaRepositoryMock.Setup(repo => repo.AddPizzaAsync(It.IsAny<WebApi.Data.Models.Pizza>())).ReturnsAsync(newPizza);
                var addedPizza = await pizzaService.AddPizzaAsync(newPizza, user.Id);

                Assert.Equal(newPizza, addedPizza);

            }


            [Fact]
            public async Task UpdatePizzaAsync_ShouldUpdatePizzaForAdmin()
            {
                // Arrange
                var adminUserId = "2";
                var adminUser = new User { Id = adminUserId, Email = "admin_1gmail.com", FullName = "Admin A", UserName = "adminA" };
           

                var userManagerMock = MockUserManager.CreateMockUserManager(adminUser, "Admin");
                var pizzaRepositoryMock = new Mock<IPizzaRepository>();
                var pizzaService = new PizzaService(pizzaRepositoryMock.Object, userManagerMock.Object);

                var pizzaId = 1;
                var originalPizza = new WebApi.Data.Models.Pizza
                {
                    Id = pizzaId,
                    CreatedBy = adminUserId,
                    IsGlutenFree = false,
                    Name = "Hawayana",
                    Created = DateTime.UtcNow,
                    Price = 2,
                    Size = PizzaSize.Small
                };

                var updatedPizza = new WebApi.Data.Models.Pizza
                {
                    Id = pizzaId,
                    CreatedBy = adminUserId,
                    IsGlutenFree = true,
                    Name = "Hawayana 2",
                    Created = DateTime.UtcNow,
                    Price = 3,
                    Size = PizzaSize.Medium
                };

                pizzaRepositoryMock.Setup(repo => repo.GetPizzaAsync(pizzaId)).ReturnsAsync(originalPizza);
                pizzaRepositoryMock.Setup(repo => repo.UpdatePizzaAsync(originalPizza)).ReturnsAsync(updatedPizza);

                // Act
                var result = await pizzaService.UpdatePizzaAsync(pizzaId, originalPizza, adminUserId);

                // Assert
                Assert.NotNull(result);
                Assert.NotEqual(originalPizza, result); // Verificar que la pizza devuelta no sea igual a la original

                // Verify that UpdatePizzaAsync method of the repository was called
                pizzaRepositoryMock.Verify(repo => repo.UpdatePizzaAsync(originalPizza), Times.Once);
            }

            [Fact]
            public async Task UpdatePizzaAsync_ShouldUpdatePizzaForUser()
            {
                // Arrange
                var userId = "6";
                var user = new User { Id = userId, Email = "user45@gmail.com", FullName = "Usuario 46", UserName = "user46" };


                var userManagerMock = MockUserManager.CreateMockUserManager(user, "User");
                var pizzaRepositoryMock = new Mock<IPizzaRepository>();
                var pizzaService = new PizzaService(pizzaRepositoryMock.Object, userManagerMock.Object);

                var pizzaId = 4;
                var originalPizza = new WebApi.Data.Models.Pizza
                {
                    Id = pizzaId,
                    CreatedBy = userId,
                    IsGlutenFree = false,
                    Name = "Hawayana",
                    Created = DateTime.UtcNow,
                    Price = 2,
                    Size = PizzaSize.Small
                };

                var updatedPizza = new WebApi.Data.Models.Pizza
                {
                    Id = pizzaId,
                    CreatedBy = userId,
                    IsGlutenFree = true,
                    Name = "Hawayana large",
                    Created = DateTime.UtcNow,
                    Price = 4,
                    Size = PizzaSize.Large
                };

                pizzaRepositoryMock.Setup(repo => repo.GetPizzaAsync(pizzaId)).ReturnsAsync(originalPizza);
                pizzaRepositoryMock.Setup(repo => repo.UpdatePizzaAsync(originalPizza)).ReturnsAsync(updatedPizza);

                // Act
                var result = await pizzaService.UpdatePizzaAsync(pizzaId, originalPizza, userId);

                // Assert
                Assert.NotNull(result);
                Assert.NotEqual(originalPizza, result); // Verificar que la pizza devuelta no sea igual a la original

                // Verify that UpdatePizzaAsync method of the repository was called
                pizzaRepositoryMock.Verify(repo => repo.UpdatePizzaAsync(originalPizza), Times.Once);
            }



            [Fact]
            public async Task DeletePizzaAsync_ShouldDeletePizzaForAdmin()
            {
                var userId = "3";
                var adminUser = new User { Id = userId, Email = "admin_2gmail.com", FullName = "Admin B", UserName = "adminB" };

                var userManagerMock = MockUserManager.CreateMockUserManager(adminUser, "Admin");
                var pizzaRepositoryMock = new Mock<IPizzaRepository>();
                var pizzaService = new PizzaService(pizzaRepositoryMock.Object, userManagerMock.Object);

                var pizzaId = 1;
                var pizzaToDelete = new WebApi.Data.Models.Pizza
                {
                    Id = pizzaId,
                    CreatedBy = userId,
                    IsGlutenFree = false,
                    Name = "Peperoni",
                    Created = DateTime.UtcNow,
                    Price = 3,
                    Size = PizzaSize.Medium
                };

                pizzaRepositoryMock.Setup(repo => repo.GetPizzaAsync(pizzaId)).ReturnsAsync(pizzaToDelete);

                // Act
                await pizzaService.DeletePizzaAsync(pizzaId, adminUser.Id);

                // Assert
                // Verify that DeletePizzaAsync method of the repository was called
                pizzaRepositoryMock.Verify(repo => repo.DeletePizzaAsync(pizzaId), Times.Once);


            }


            [Fact]
            public async Task DeletePizzaAsync_ShouldDeletePizzaForUser()
            {
                var userId = "9";
                var user = new User { Id = userId, Email = "userG@gmail.com", FullName = "Usuario G", UserName = "userG" };

                var userManagerMock = MockUserManager.CreateMockUserManager(user, "User");
                var pizzaRepositoryMock = new Mock<IPizzaRepository>();
                var pizzaService = new PizzaService(pizzaRepositoryMock.Object, userManagerMock.Object);

                var pizzaId = 8;
                var pizzaToDelete = new WebApi.Data.Models.Pizza
                {
                    Id = pizzaId,
                    CreatedBy = userId,
                    IsGlutenFree = true,
                    Name = "Tocino",
                    Created = DateTime.UtcNow,
                    Price = 2,
                    Size = PizzaSize.Medium
                };

                pizzaRepositoryMock.Setup(repo => repo.GetPizzaAsync(pizzaId)).ReturnsAsync(pizzaToDelete);

                // Act
                await pizzaService.DeletePizzaAsync(pizzaId, user.Id);

                // Assert
                // Verify that DeletePizzaAsync method of the repository was called
                pizzaRepositoryMock.Verify(repo => repo.DeletePizzaAsync(pizzaId), Times.Once);


            }

        }

    }

}
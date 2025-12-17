using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using WebProject_klas3_groep4;
using WebProject_klas3_groep4.Controllers;
using WebProject_klas3_groep4.models;
using WebProject_klas3_groep4.DTO;
using WebProject_klas3_groep4.Tests.TestHelpers;
using Xunit;

namespace WebProject_klas3_groep4.Tests
{
    public class ProductControllerTests
    {
        private DatabaseContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new DatabaseContext(options);
        }

        private Mock<UserManager<GebruikerDB>> CreateFakeUserManager()
        {
            var store = new Mock<IUserStore<GebruikerDB>>();
            var mgr = new Mock<UserManager<GebruikerDB>>(
                store.Object, null, null, null, null, null, null, null, null
            );
            return mgr;
        }

        private ProductController CreateControllerWithUser(
            DatabaseContext context,
            GebruikerDB user)
        {
            var userManagerMock = CreateFakeUserManager();

            userManagerMock
                .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var controller = new ProductController(context, userManagerMock.Object);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) },
                            "TestAuth"
                        )
                    )
                }
            };

            return controller;
        }

        // ---------------------------------------------------------
        // TEST 1: GetProducten
        // ---------------------------------------------------------
        [Fact]
        public async Task GetProducten_ReturnsOk_WithListOfProducts()
        {
            using var context = CreateInMemoryDbContext();

            context.Producten.Add(TestSpawner.CreateValidProduct(1));
            context.Producten.Add(TestSpawner.CreateValidProduct(2));
            await context.SaveChangesAsync();

            var fakeUser = new GebruikerDB { Id = 1, UserName = "testuser" };
            var controller = CreateControllerWithUser(context, fakeUser);

            var result = await controller.GetProducten();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsAssignableFrom<IEnumerable<ProductOutputDto>>(okResult.Value);

            Assert.Equal(2, data.Count());
        }

        // ---------------------------------------------------------
        // TEST 2: PostProduct
        // ---------------------------------------------------------
        [Fact]
        public async Task PostProduct_CreatesProduct_ForLoggedInUser()
        {
            using var context = CreateInMemoryDbContext();

            var fakeUser = new GebruikerDB { Id = 123, UserName = "aanvoerder1" };
            var controller = CreateControllerWithUser(context, fakeUser);

            var dto = new ProductCreateDto
            {
                Naam = "Nieuwe Roos",
                Beschrijving = "Mooie rode roos",
                Foto = null,
                Oogstdatum = DateTime.UtcNow,
                Potmaat = 10,
                Gewicht = 2.5,
                Steellengte = 50,
                Hoeveelheid = 100,
                MinimalePrijs = 2,
                VeilingId = null
            };

            var result = await controller.PostProduct(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var outDto = Assert.IsType<ProductOutputDto>(createdResult.Value);

            Assert.Equal("Nieuwe Roos", outDto.Naam);
            Assert.Equal(fakeUser.Id, outDto.AanvoerderId);

            var productInDb = await context.Producten.FirstOrDefaultAsync(p => p.ID == outDto.Id);
            Assert.NotNull(productInDb);
            Assert.Equal(fakeUser.Id, productInDb.AanvoerderId);
        }

        // ---------------------------------------------------------
        // TEST 3: Delete blokkeert bij verkeerde eigenaar
        // ---------------------------------------------------------
        [Fact]
        public async Task DeleteProduct_ReturnsForbid_IfUserIsNotOwner()
        {
            using var context = CreateInMemoryDbContext();

            context.Producten.Add(TestSpawner.CreateValidProduct(id: 1, aanvoerderId: 999));
            await context.SaveChangesAsync();

            var fakeUser = new GebruikerDB { Id = 123 };
            var controller = CreateControllerWithUser(context, fakeUser);

            var result = await controller.DeleteProduct(1);

            Assert.IsType<ForbidResult>(result);
        }
    }
}
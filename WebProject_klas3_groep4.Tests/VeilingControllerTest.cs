using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using WebProject_klas3_groep4.Controllers;
using WebProject_klas3_groep4.models;
using WebProject_klas3_groep4.DTO;
using WebProject_klas3_groep4.Tests.Infrastructure;
using Xunit;

namespace WebProject_klas3_groep4.Tests
{
    public class VeilingControllerTests
    {
        private DatabaseContext CreateDb()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new DatabaseContext(options);
        }

        private Mock<UserManager<GebruikerDB>> FakeUserManager()
        {
            var store = new Mock<IUserStore<GebruikerDB>>();
            return new Mock<UserManager<GebruikerDB>>(
                store.Object, null, null, null, null, null, null, null, null
            );
        }

        private VeilingController CreateControllerWithUser(DatabaseContext db, GebruikerDB user)
        {
            var um = FakeUserManager();

            um.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
              .ReturnsAsync(user);

            var controller = new VeilingController(db, um.Object);

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
        // TEST 1 — GET ALL
        [Fact]
        public async Task GetVeilingen_ReturnsList()
        {
            using var db = CreateDb();

            db.Veilingen.Add(TestSpawner.CreateValidVeiling(1));
            db.Veilingen.Add(TestSpawner.CreateValidVeiling(2));
            await db.SaveChangesAsync();

            var controller = CreateControllerWithUser(db, new GebruikerDB { Id = 10 });

            var result = await controller.GetVeilingen();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsAssignableFrom<IEnumerable<VeilingOutputDto>>(ok.Value);

            Assert.Equal(2, list.Count());
        }

        // ---------------------------------------------------------
        // TEST 2 — GET BY ID
        [Fact]
        public async Task GetVeiling_ReturnsNotFound_IfMissing()
        {
            using var db = CreateDb();

            var controller = CreateControllerWithUser(db, new GebruikerDB { Id = 10 });

            var result = await controller.GetVeiling(99);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetVeiling_ReturnsDto_IfExists()
        {
            using var db = CreateDb();

            db.Veilingen.Add(TestSpawner.CreateValidVeiling(5));
            await db.SaveChangesAsync();

            var controller = CreateControllerWithUser(db, new GebruikerDB { Id = 10 });

            var result = await controller.GetVeiling(5);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<VeilingOutputDto>(ok.Value);

            Assert.Equal(5, dto.Id);
        }

        // ---------------------------------------------------------
        // TEST 3 — POST
        [Fact]
        public async Task PostVeiling_CreatesVeiling()
        {
            using var db = CreateDb();

            var controller = CreateControllerWithUser(db, new GebruikerDB { Id = 42 });

            var dto = new VeilingCreateDto
            {
                KlokLocatie = "Zaal 7",
                AantalProducten = 20,
                HuidigeSituatieVanVeiling = "Open",
                Bechrijving = "Test veiling"
            };

            var result = await controller.PostVeiling(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var outDto = Assert.IsType<VeilingOutputDto>(created.Value);

            Assert.Equal("Zaal 7", outDto.KlokLocatie);
            Assert.Equal(42, outDto.VeilingmeesterId);
        }

        // ---------------------------------------------------------
        // TEST 4 — PUT
        [Fact]
        public async Task PutVeiling_Forbid_WhenNotOwner()
        {
            using var db = CreateDb();

            db.Veilingen.Add(TestSpawner.CreateValidVeiling(id: 11, veilingmeesterId: 999));
            await db.SaveChangesAsync();

            var controller = CreateControllerWithUser(db, new GebruikerDB { Id = 100 });

            var res = await controller.PutVeiling(11, new VeilingUpdateDto { KlokLocatie = "Nieuwe locatie" });

            Assert.IsType<ForbidResult>(res.Result);
        }

        [Fact]
        public async Task PutVeiling_Updates_WhenOwner()
        {
            using var db = CreateDb();

            db.Veilingen.Add(TestSpawner.CreateValidVeiling(id: 12, veilingmeesterId: 50));
            await db.SaveChangesAsync();

            var controller = CreateControllerWithUser(db, new GebruikerDB { Id = 50 });

            var res = await controller.PutVeiling(12, new VeilingUpdateDto { KlokLocatie = "Nieuw" });

            var ok = Assert.IsType<OkObjectResult>(res.Result);

            var updated = await db.Veilingen.FindAsync(12);
            Assert.Equal("Nieuw", updated.KlokLocatie);
        }

        // ---------------------------------------------------------
        // TEST 5 — DELETE
        [Fact]
        public async Task DeleteVeiling_Forbid_WhenNotOwner()
        {
            using var db = CreateDb();

            db.Veilingen.Add(TestSpawner.CreateValidVeiling(id: 13, veilingmeesterId: 999));
            await db.SaveChangesAsync();

            var controller = CreateControllerWithUser(db, new GebruikerDB { Id = 88 });

            var res = await controller.DeleteVeiling(13);

            Assert.IsType<ForbidResult>(res);
        }

        [Fact]
        public async Task DeleteVeiling_Removes_WhenOwner()
        {
            using var db = CreateDb();

            db.Veilingen.Add(TestSpawner.CreateValidVeiling(id: 14, veilingmeesterId: 30));
            await db.SaveChangesAsync();

            var controller = CreateControllerWithUser(db, new GebruikerDB { Id = 30 });

            var res = await controller.DeleteVeiling(14);

            Assert.IsType<NoContentResult>(res);
            Assert.False(db.Veilingen.Any(v => v.ID == 14));
        }

        // ---------------------------------------------------------
        // TEST 6 — GET BY VEILINGMEESTER
        [Fact]
        public async Task GetVeilingenByVeilingmeester_FiltersCorrect()
        {
            using var db = CreateDb();

            db.Veilingen.Add(TestSpawner.CreateValidVeiling(id: 1, veilingmeesterId: 10));
            db.Veilingen.Add(TestSpawner.CreateValidVeiling(id: 2, veilingmeesterId: 10));
            db.Veilingen.Add(TestSpawner.CreateValidVeiling(id: 3, veilingmeesterId: 22));
            await db.SaveChangesAsync();

            var controller = CreateControllerWithUser(db, new GebruikerDB { Id = 10 });

            var result = await controller.GetVeilingenByVeilingmeester(10);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsAssignableFrom<IEnumerable<VeilingOutputDto>>(ok.Value);

            Assert.Equal(2, list.Count());
        }
    }
}

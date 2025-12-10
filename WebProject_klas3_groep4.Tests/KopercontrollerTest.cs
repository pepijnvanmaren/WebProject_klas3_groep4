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
    public class KopercontrollerTest
    {
        private readonly Mock<UserManager<GebruikerDB>> _userManagerMock;
        private readonly DatabaseContext _context;
        private readonly KoperController _controller;

        public KopercontrollerTest()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new DatabaseContext(options);
            var userStoreMock = new Mock<IUserStore<GebruikerDB>>();
            _userManagerMock = new Mock<UserManager<GebruikerDB>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _controller = new KoperController(_context, _userManagerMock.Object);
        }

        // ------------------------------------------------------------
        // GET ALL TESTS
        // ------------------------------------------------------------
        [Fact]
        public async Task GetKoper_ReturnsAllKopers()
        {
            // Arrange
            var Kopper1 = TestSpawner.CreateValidKoper(id: 1, rol: "Koper", userName: "Kopper1");
            var Kopper2 = TestSpawner.CreateValidKoper(id: 2, rol: "Koper", userName: "Kopper2");

            _context.Gebruikers.AddRange(Kopper1, Kopper2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetKopers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var kopers = Assert.IsAssignableFrom<List<GebruikerDto>>(okResult.Value);
            Assert.Equal(2, kopers.Count());

        }

        [Fact]
        public async Task GetAanvoerders_NoKopers_ReturnsEmptyList()
        {
            // Act
            var result = await _controller.GetKopers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var Kopers = Assert.IsAssignableFrom<List<GebruikerDto>>(okResult.Value);
            Assert.Empty(Kopers);
        }

        // ------------------------------------------------------------
        // GET SINGLE TESTS
        // ------------------------------------------------------------

        [Fact]
        public async Task GetKoper_ReturnsGebruiker_WhenExists()
        {
            // Arrange
            var Koper = TestSpawner.CreateValidKoper(1, "test@test.nl");

            _userManagerMock.Setup(x => x.FindByIdAsync("1"))
                .ReturnsAsync(Koper);

            // Act
            var result = await _controller.GetKoper(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<GebruikerDto>(okResult.Value);
            Assert.Equal(Koper.Id, dto.Id);
            Assert.Equal(Koper.Email, dto.Email);
        }

        [Fact]
        public async Task GetKoper_ReturnsNotFound_WhenDoesNotExist()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByIdAsync("3"))
                .ReturnsAsync((GebruikerDB)null);

            // Act
            var result = await _controller.GetKoper(3);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }


        // ------------------------------------------------------------
        // CREATE TESTS
        // ------------------------------------------------------------

        [Fact]
        public async Task PostKoper_CreatesGebruiker_WhenValid()
        {
            // Arrange
            var createDto = TestSpawner.CreateValidKoperCreateDto();

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<GebruikerDB>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<GebruikerDB, string>((g, p) => g.Id = 1);

            // Act
            var result = await _controller.PostKoper(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var dto = Assert.IsType<GebruikerDto>(createdResult.Value);
            Assert.Equal(createDto.Email, dto.Email);
            Assert.Equal(createDto.UserName, dto.UserName);
        }
        [Fact]
        public async Task PostKoper_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = await _controller.PostKoper(null);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async Task PostKoper_ReturnsBadRequest_WhenCreationFails()
        {
            // Arrange
            var createDto = TestSpawner.CreateValidKoperCreateDto();
            var errors = new[] { new IdentityError { Description = "Password too weak" } };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<GebruikerDB>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(errors));

            // Act
            var result = await _controller.PostKoper(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequestResult.Value);
        }

        // ------------------------------------------------------------
        // UPDATE TESTS
        // ------------------------------------------------------------
        [Fact]
        public async Task PutKoper_UpdatesGebruiker_WhenValid()
        {
            // Arrange
            var gebruiker = TestSpawner.CreateValidKoper(1, "old@test.nl");
            var updateDto = TestSpawner.CreateValidKoperUpdateDto(1, "new@test.nl");

            // Mock authenticated user
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
             new Claim(ClaimTypes.NameIdentifier, "1")
            }, "TestAuth"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Mock UserManager
            _userManagerMock.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(gebruiker);

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                            .ReturnsAsync(gebruiker);

            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<GebruikerDB>()))
                            .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.PutKoper(1, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<GebruikerDto>(okResult.Value);

            Assert.Equal(updateDto.Email, dto.Email);
        }

        [Fact]
        public async Task PutKoper_ReturnsNotFound_WhenGebruikerDoesNotExist()
        {
            // Arrange
            var updateDto = TestSpawner.CreateValidKoperUpdateDto();

            _userManagerMock.Setup(x => x.FindByIdAsync("999"))
                .ReturnsAsync((GebruikerDB)null);

            // Act
            var result = await _controller.PutKoper(999, updateDto);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        // ------------------------------------------------------------
        // DELETE TESTS
        // ------------------------------------------------------------

        [Fact]
        public async Task DeleteKoper_DeletesGebruiker_WhenExists()
        {
            // Arrange
            var gebruiker = TestSpawner.CreateValidKoper(1, "test@test.nl");

            _userManagerMock.Setup(x => x.FindByIdAsync("1"))
                .ReturnsAsync(gebruiker);
            _userManagerMock.Setup(x => x.DeleteAsync(It.IsAny<GebruikerDB>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.DeleteKoper(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _userManagerMock.Verify(x => x.DeleteAsync(gebruiker), Times.Once);
        }
        [Fact]
        public async Task DeleteKoper_ReturnsNotFound_WhenGebruikerDoesNotExist()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByIdAsync("21"))
                .ReturnsAsync((GebruikerDB)null);

            // Act
            var result = await _controller.DeleteKoper(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}

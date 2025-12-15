using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using WebProject_klas3_groep4.Controllers;
using WebProject_klas3_groep4.models;
using WebProject_klas3_groep4.DTO;
using WebProject_klas3_groep4.Tests.Infrastructure;
using Xunit;

namespace WebProject_klas3_groep4.Tests
{
    public class GebruikerControllerTest
    {
        private readonly Mock<UserManager<GebruikerDB>> _mockUserManager;
        private readonly DatabaseContext _context;
        private readonly GebruikersController _controller;

        public GebruikerControllerTest()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new DatabaseContext(options);

            // Setup mock UserManager
            var userStoreMock = new Mock<IUserStore<GebruikerDB>>();
            _mockUserManager = new Mock<UserManager<GebruikerDB>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            _controller = new GebruikersController(_context, _mockUserManager.Object);
        }

        // ------------------------------------------------------------
        // TEST 1 - GET ALL TESTS
        [Fact]
        public async Task GetGebruikers_ReturnsAllGebruikers()
        {
            // Arrange
            var gebruiker1 = TestSpawner.CreateValidGebruiker(1, "user1@test.nl");
            var gebruiker2 = TestSpawner.CreateValidGebruiker(2, "user2@test.nl");

            _context.Gebruikers.AddRange(gebruiker1, gebruiker2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetGebruikers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var gebruikers = Assert.IsAssignableFrom<IEnumerable<GebruikerDto>>(okResult.Value);
            Assert.Equal(2, gebruikers.Count());
        }

        [Fact]
        public async Task GetGebruikers_ReturnsEmptyList_WhenNoGebruikers()
        {
            // Act
            var result = await _controller.GetGebruikers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var gebruikers = Assert.IsAssignableFrom<IEnumerable<GebruikerDto>>(okResult.Value);
            Assert.Empty(gebruikers);
        }

        // ------------------------------------------------------------
        // TEST 2 - GET SINGLE TESTS
        [Fact]
        public async Task GetGebruiker_ReturnsGebruiker_WhenExists()
        {
            // Arrange
            var gebruiker = TestSpawner.CreateValidGebruiker(1, "test@test.nl");

            _mockUserManager.Setup(x => x.FindByIdAsync("1"))
                .ReturnsAsync(gebruiker);

            // Act
            var result = await _controller.GetGebruiker(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<GebruikerDto>(okResult.Value);
            Assert.Equal(gebruiker.Id, dto.Id);
            Assert.Equal(gebruiker.Email, dto.Email);
        }

        [Fact]
        public async Task GetGebruiker_ReturnsNotFound_WhenDoesNotExist()
        {
            // Arrange
            _mockUserManager.Setup(x => x.FindByIdAsync("999"))
                .ReturnsAsync((GebruikerDB)null);

            // Act
            var result = await _controller.GetGebruiker(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        // ------------------------------------------------------------
        // TEST 3 - CREATE TESTS
        [Fact]
        public async Task PostGebruiker_CreatesGebruiker_WhenValid()
        {
            // Arrange
            var createDto = TestSpawner.CreateValidGebruikerCreateDto();

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<GebruikerDB>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<GebruikerDB, string>((g, p) => g.Id = 1);

            // Act
            var result = await _controller.PostGebruiker(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var dto = Assert.IsType<GebruikerDto>(createdResult.Value);
            Assert.Equal(createDto.Email, dto.Email);
            Assert.Equal(createDto.UserName, dto.UserName);
        }

        [Fact]
        public async Task PostGebruiker_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = await _controller.PostGebruiker(null);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async Task PostGebruiker_ReturnsBadRequest_WhenCreationFails()
        {
            // Arrange
            var createDto = TestSpawner.CreateValidGebruikerCreateDto();
            var errors = new[] { new IdentityError { Description = "Wachtwoord is te zwak" } };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<GebruikerDB>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(errors));

            // Act
            var result = await _controller.PostGebruiker(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequestResult.Value);
        }

        // ------------------------------------------------------------
        // TEST 4 - UPDATE TESTS
        [Fact]
        public async Task PutGebruiker_UpdatesGebruiker_WhenValid()
        {
            // Arrange
            var gebruiker = TestSpawner.CreateValidGebruiker(1, "old@test.nl");
            var updateDto = TestSpawner.CreateValidGebruikerUpdateDto("new@test.nl");

            _mockUserManager.Setup(x => x.FindByIdAsync("1"))
                .ReturnsAsync(gebruiker);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<GebruikerDB>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.PutGebruiker(1, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<GebruikerDto>(okResult.Value);
            Assert.Equal(updateDto.Email, dto.Email);
        }

        [Fact]
        public async Task PutGebruiker_UpdatesPassword_WhenNewPasswordProvided()
        {
            // Arrange
            var gebruiker = TestSpawner.CreateValidGebruiker(1, "test@test.nl");
            var updateDto = TestSpawner.CreateValidGebruikerUpdateDto("test@test.nl", "NewPassword123!");

            _mockUserManager.Setup(x => x.FindByIdAsync("1"))
                .ReturnsAsync(gebruiker);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<GebruikerDB>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<GebruikerDB>()))
                .ReturnsAsync("reset-token");
            _mockUserManager.Setup(x => x.ResetPasswordAsync(It.IsAny<GebruikerDB>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.PutGebruiker(1, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            _mockUserManager.Verify(x => x.GeneratePasswordResetTokenAsync(It.IsAny<GebruikerDB>()), Times.Once);
            _mockUserManager.Verify(x => x.ResetPasswordAsync(It.IsAny<GebruikerDB>(), "reset-token", "NewPassword123!"), Times.Once);
        }

        [Fact]
        public async Task PutGebruiker_ReturnsNotFound_WhenGebruikerDoesNotExist()
        {
            // Arrange
            var updateDto = TestSpawner.CreateValidGebruikerUpdateDto();

            _mockUserManager.Setup(x => x.FindByIdAsync("999"))
                .ReturnsAsync((GebruikerDB)null);

            // Act
            var result = await _controller.PutGebruiker(999, updateDto);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        // ------------------------------------------------------------
        // TEST 5 - DELETE TESTS
        [Fact]
        public async Task DeleteGebruiker_DeletesGebruiker_WhenExists()
        {
            // Arrange
            var gebruiker = TestSpawner.CreateValidGebruiker(1, "test@test.nl");

            _mockUserManager.Setup(x => x.FindByIdAsync("1"))
                .ReturnsAsync(gebruiker);
            _mockUserManager.Setup(x => x.DeleteAsync(It.IsAny<GebruikerDB>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.DeleteGebruiker(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockUserManager.Verify(x => x.DeleteAsync(gebruiker), Times.Once);
        }

        [Fact]
        public async Task DeleteGebruiker_ReturnsNotFound_WhenGebruikerDoesNotExist()
        {
            // Arrange
            _mockUserManager.Setup(x => x.FindByIdAsync("999"))
                .ReturnsAsync((GebruikerDB)null);

            // Act
            var result = await _controller.DeleteGebruiker(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}

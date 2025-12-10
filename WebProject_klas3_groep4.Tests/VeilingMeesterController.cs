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
    public class VeilingMeestercontrollerTest
    {
        private readonly Mock<UserManager<GebruikerDB>> _userManagerMock;
        private readonly DatabaseContext _context;
        private readonly VeilingmeesterController _controller;

        public VeilingMeestercontrollerTest()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new DatabaseContext(options);
            var userStoreMock = new Mock<IUserStore<GebruikerDB>>();
            _userManagerMock = new Mock<UserManager<GebruikerDB>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _controller = new VeilingmeesterController(_context, _userManagerMock.Object);
        }

        // ------------------------------------------------------------
        // GET ALL TESTS
        // ------------------------------------------------------------
        [Fact]
        public async Task GetVeilingmeester_ReturnsAllVeilingmeesters()
        {
            // Arrange
            var Veilingmeester1 = TestSpawner.CreateValidVeilingmeester(id: 1, rol: "Veilingmeester", userName: "Veilingmeester1");
            var Veilingmeester2 = TestSpawner.CreateValidVeilingmeester(id: 2, rol: "Veilingmeester", userName: "Veilingmeester2");

            _context.Gebruikers.AddRange(Veilingmeester1, Veilingmeester2);
            await _context.SaveChangesAsync();

            // Act
            var result = _controller.GetVeilingmeesters();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var Veilingmeesters = Assert.IsAssignableFrom<List<VeilingmeesterOutputDto>>(okResult.Value);
            Assert.Equal(2, Veilingmeesters.Count());

        }

        [Fact]
        public async Task GetVeilingmeesters_NoVeilingmeesters_ReturnsEmptyList()
        {
            // Act
            var result = _controller.GetVeilingmeesters();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var Veilingmeesters = Assert.IsAssignableFrom<List<VeilingmeesterOutputDto>>(okResult.Value);
            Assert.Empty(Veilingmeesters);
        }

        // ------------------------------------------------------------
        // GET SINGLE TESTS
        // ------------------------------------------------------------

        [Fact]
        public async Task GetVeilingmeester_ReturnsGebruiker_WhenExists()
        {
            // Arrange
            var Veilingmeester = TestSpawner.CreateValidVeilingmeester(1, "test@test.nl");

            _userManagerMock.Setup(x => x.FindByIdAsync("1"))
                .ReturnsAsync(Veilingmeester);

            // Act
            var result = await _controller.GetVeilingmeester(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<GebruikerDto>(okResult.Value);
            Assert.Equal(Veilingmeester.Id, dto.Id);
            Assert.Equal(Veilingmeester.Email, dto.Email);
        }

        [Fact]
        public async Task GetVeilingmeester_ReturnsNotFound_WhenDoesNotExist()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByIdAsync("3"))
                .ReturnsAsync((GebruikerDB)null);

            // Act
            var result = await _controller.GetVeilingmeester(3);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }


        // ------------------------------------------------------------
        // CREATE TESTS
        // ------------------------------------------------------------

        [Fact]
        public async Task PostVeilingmeester_CreatesGebruiker_WhenValid()
        {
            // Arrange
            var createDto = TestSpawner.CreateValidVeilingmeesterCreateDto();

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<GebruikerDB>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<GebruikerDB, string>((g, p) => g.Id = 1);

            // Act
            var result = await _controller.PostVeilingmeester(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var dto = Assert.IsType<VeilingmeesterOutputDto>(createdResult.Value);
            Assert.Equal(createDto.Email, dto.Email);
            Assert.Equal(createDto.UserName, dto.UserName);
        }
        [Fact]
        public async Task PostVeilingmeester_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = await _controller.PostVeilingmeester(null);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async Task PostVeilingmeester_ReturnsBadRequest_WhenCreationFails()
        {
            // Arrange
            var createDto = TestSpawner.CreateValidVeilingmeesterCreateDto();
            var errors = new[] { new IdentityError { Description = "Password too weak" } };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<GebruikerDB>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(errors));

            // Act
            var result = await _controller.PostVeilingmeester(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequestResult.Value);
        }

        // ------------------------------------------------------------
        // UPDATE TESTS
        // ------------------------------------------------------------
        [Fact]
        public async Task PutVeilingmeester_UpdatesGebruiker_WhenValid()
        {
            // Arrange
            var gebruiker = TestSpawner.CreateValidVeilingmeester(1, "old@test.nl");
            var updateDto = TestSpawner.CreateValidVeilingmeesterUpdateDto(1, "new@test.nl");

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
            var result = await _controller.PutVeilingmeester(1, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<GebruikerDto>(okResult.Value);

            Assert.Equal(updateDto.Email, dto.Email);
        }

        [Fact]
        public async Task PutVeilingmeester_ReturnsNotFound_WhenGebruikerDoesNotExist()
        {
            // Arrange
            var updateDto = TestSpawner.CreateValidVeilingmeesterUpdateDto();

            _userManagerMock.Setup(x => x.FindByIdAsync("999"))
                .ReturnsAsync((GebruikerDB)null);

            // Act
            var result = await _controller.PutVeilingmeester(999, updateDto);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        // ------------------------------------------------------------
        // DELETE TESTS
        // ------------------------------------------------------------

        [Fact]
        public async Task DeleteVeilingmeester_DeletesGebruiker_WhenExists()
        {
            // Arrange
            var gebruiker = TestSpawner.CreateValidVeilingmeester(1, "test@test.nl");

            _userManagerMock.Setup(x => x.FindByIdAsync("1"))
                .ReturnsAsync(gebruiker);
            _userManagerMock.Setup(x => x.DeleteAsync(It.IsAny<GebruikerDB>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.DeleteVeilingmeester(1);

            // Assert
            Assert.IsType<OkResult>(result); // <-- Not NotFound
            _userManagerMock.Verify(x => x.DeleteAsync(gebruiker), Times.Once);
        }
        [Fact]
        public async Task DeleteVeilingmeester_ReturnsNotFound_WhenGebruikerDoesNotExist()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByIdAsync("21"))
                .ReturnsAsync((GebruikerDB)null);

            // Act
            var result = await _controller.DeleteVeilingmeester(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}

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
    public class AanvoerderControllerTest
    {
        private readonly Mock<UserManager<GebruikerDB>> _userManagerMock;
        private readonly DatabaseContext _context;
        private readonly AanvoerderController _controller;

        public AanvoerderControllerTest()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new DatabaseContext(options);
            var userStoreMock = new Mock<IUserStore<GebruikerDB>>();
            _userManagerMock = new Mock<UserManager<GebruikerDB>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _controller = new AanvoerderController(_context, _userManagerMock.Object);
        }

        // ------------------------------------------------------------
        // GET ALL TESTS
        // ------------------------------------------------------------
        [Fact]
        public async Task GetAanvoerders_ReturnsAllAanvoerders()
        {
            // Arrange
            var aanvoerder1 = TestSpawner.CreateValidAanvoerder(id: 1, rol: "Aanvoerder", userName: "Aanvoerder1");
            var aanvoerder2 = TestSpawner.CreateValidAanvoerder(id: 2, rol: "Aanvoerder", userName: "Aanvoerder2");

            _context.Gebruikers.AddRange(aanvoerder1, aanvoerder2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetAanvoerders();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var aanvoerders = Assert.IsAssignableFrom<List<AanvoerderOutputDto>>(okResult.Value);
            Assert.Equal(2, aanvoerders.Count());

        }

        [Fact]
        public async Task GetAanvoerders_NoAanvoerders_ReturnsEmptyList()
        {
            // Act
            var result = await _controller.GetAanvoerders();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var aanvoerders = Assert.IsAssignableFrom<List<AanvoerderOutputDto>>(okResult.Value);
            Assert.Empty(aanvoerders);
        }

        // ------------------------------------------------------------
        // GET SINGLE TESTS
        // ------------------------------------------------------------

        [Fact]
        public async Task GetAanvoerder_ReturnsGebruiker_WhenExists()
        {
            // Arrange
            var aanvoerder = TestSpawner.CreateValidAanvoerder(1, "test@test.nl");

            _userManagerMock.Setup(x => x.FindByIdAsync("1"))
                .ReturnsAsync(aanvoerder);

            // Act
            var result = await _controller.GetAanvoerder(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<AanvoerderOutputDto>(okResult.Value);
            Assert.Equal(aanvoerder.Id, dto.Id);
            Assert.Equal(aanvoerder.Email, dto.Email);
        }

        [Fact]
        public async Task GetAanvoerder_ReturnsNotFound_WhenDoesNotExist()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByIdAsync("3"))
                .ReturnsAsync((GebruikerDB)null);

            // Act
            var result = await _controller.GetAanvoerder(3);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }


        // ------------------------------------------------------------
        // CREATE TESTS
        // ------------------------------------------------------------

        [Fact]
        public async Task PostAaanvoerder_CreatesGebruiker_WhenValid()
        {
            // Arrange
            var createDto = TestSpawner.CreateValidAanvoerderCreateDto();

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<GebruikerDB>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<GebruikerDB, string>((g, p) => g.Id = 1);

            // Act
            var result = await _controller.PostAanvoerder(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var dto = Assert.IsType<AanvoerderOutputDto>(createdResult.Value);
            Assert.Equal(createDto.Email, dto.Email);
            Assert.Equal(createDto.UserName, dto.UserName);
        }
        [Fact]
        public async Task PostAaanvoerder_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = await _controller.PostAanvoerder(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task PostAaanvoerder_ReturnsBadRequest_WhenCreationFails()
        {
            // Arrange
            var createDto = TestSpawner.CreateValidAanvoerderCreateDto();
            var errors = new[] { new IdentityError { Description = "Password too weak" } };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<GebruikerDB>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(errors));

            // Act
            var result = await _controller.PostAanvoerder(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequestResult.Value);
        }

        // ------------------------------------------------------------
        // UPDATE TESTS
        // ------------------------------------------------------------
        [Fact]
        public async Task PutAanvoerder_UpdatesGebruiker_WhenValid()
        {
            // Arrange
            var gebruiker = TestSpawner.CreateValidAanvoerder(1, "old@test.nl");
            var updateDto = TestSpawner.CreateValidAanvoerderUpdateDto("new@test.nl");

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
            var result = await _controller.PutAanvoerder(1, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<AanvoerderOutputDto>(okResult.Value);

            Assert.Equal(updateDto.Email, dto.Email);
        }

        [Fact]
        public async Task PutAanvoerder_ReturnsNotFound_WhenGebruikerDoesNotExist()
        {
            // Arrange
            var updateDto = TestSpawner.CreateValidAanvoerderUpdateDto();

            _userManagerMock.Setup(x => x.FindByIdAsync("999"))
                .ReturnsAsync((GebruikerDB)null);

            // Act
            var result = await _controller.PutAanvoerder(999, updateDto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }
        // ------------------------------------------------------------
        // DELETE TESTS
        // ------------------------------------------------------------

        [Fact]
        public async Task DeleteAanvoerder_DeletesGebruiker_WhenExists()
        {
            // Arrange
            var gebruiker = TestSpawner.CreateValidAanvoerder(1, "test@test.nl");

            _userManagerMock.Setup(x => x.FindByIdAsync("1"))
                .ReturnsAsync(gebruiker);
            _userManagerMock.Setup(x => x.DeleteAsync(It.IsAny<GebruikerDB>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.DeleteAanvoerder(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _userManagerMock.Verify(x => x.DeleteAsync(gebruiker), Times.Once);
        }
        [Fact]
        public async Task DeleteAanvoerder_ReturnsNotFound_WhenGebruikerDoesNotExist()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByIdAsync("21"))
                .ReturnsAsync((GebruikerDB)null);

            // Act
            var result = await _controller.DeleteAanvoerder(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
    }

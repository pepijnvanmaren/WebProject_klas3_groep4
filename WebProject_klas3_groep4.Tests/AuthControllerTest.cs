using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebProject_klas3_groep4.Controllers;
using WebProject_klas3_groep4.models;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace WebProject_klas3_groep4.Tests
{
    public class AuthControllerTest
    {
        private Mock<UserManager<GebruikerDB>> CreateUserManager()
        {
            var store = new Mock<IUserStore<GebruikerDB>>();
            return new Mock<UserManager<GebruikerDB>>(
                store.Object, null, null, null, null, null, null, null, null
            );
        }

        private Mock<SignInManager<GebruikerDB>> CreateSignInManager(UserManager<GebruikerDB> userManager)
        {
            return new Mock<SignInManager<GebruikerDB>>(
                userManager,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<GebruikerDB>>().Object,
                null, null, null, null
            );
        }

        // ---------------------------
        // TEST 1 - Login succesvol
        [Fact]
        public async Task Login_ReturnsOk_WhenCredentialsAreCorrect()
        {
            // Arrange
            var user = new GebruikerDB
            {
                Id = 1,
                Email = "test@test.nl",
                UserName = "testuser"
            };

            var userManagerMock = CreateUserManager();
            userManagerMock
                .Setup(u => u.FindByEmailAsync(user.Email))
                .ReturnsAsync(user);

            var signInManagerMock = CreateSignInManager(userManagerMock.Object);
            signInManagerMock
                .Setup(s => s.PasswordSignInAsync(user,"1234",true,false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var configMock = new Mock<IConfiguration>();

            var controller = new AuthController(
              userManagerMock.Object,
               configMock.Object
            );


            var dto = new LoginDto
            {
                Email = user.Email,
                Password = "1234"
            };

            // Act
            var result = await controller.Login(dto);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
        }

        // ---------------------------
        // TEST 2 - Login fout wachtwoord
        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenPasswordIsWrong()
        {
            // Arrange
            var user = new GebruikerDB
            {
                Id = 1,
                Email = "test@test.nl",
                UserName = "testuser"
            };

            var userManagerMock = CreateUserManager();
            var signInManagerMock = CreateSignInManager(userManagerMock.Object);

            userManagerMock
                .Setup(x => x.FindByEmailAsync(user.Email))
                .ReturnsAsync(user);

            signInManagerMock
                .Setup(x => x.PasswordSignInAsync(user, "fout", true, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var configMock = new Mock<IConfiguration>();

            var controller = new AuthController(
                userManagerMock.Object,
                   configMock.Object
            );

            var dto = new LoginDto
            {
                Email = user.Email,
                Password = "fout"
            };

            // Act
            var result = await controller.Login(dto);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}

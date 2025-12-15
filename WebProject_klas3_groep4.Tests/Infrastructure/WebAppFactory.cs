using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace WebProject_klas3_groep4.Tests.Infrastructure
{
    public class WebAppFactory : WebApplicationFactory<InvalidProgramException>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {

                // Verwijder echte database
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>)
                );
                if (descriptor != null)
                    services.Remove(descriptor);

                services.RemoveAll(typeof(DbContextOptions<DatabaseContext>));

                // Voeg in-memory database toe
                services.AddDbContext<DatabaseContext>(options =>
                {
                    options.UseInMemoryDatabase("IntegrationTestDB");
                });

                // Verwijder alle authentication
                services.RemoveAll(typeof(Microsoft.AspNetCore.Authentication.IAuthenticationService));
                services.RemoveAll(typeof(Microsoft.AspNetCore.Authorization.IAuthorizationHandler));
                services.RemoveAll(typeof(Microsoft.AspNetCore.Authorization.IAuthorizationPolicyProvider));

                // Zorg dat Authorize ALTIJD true teruggeeft
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("AllowAll", policy =>
                        policy.RequireAssertion(_ => true));
                });

                services.AddAuthentication("Test")
                    .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions,
                               TestStaAnoniemToe>("Test", o => { });
            });

            // Forceer altijd Test-auth scheme
            builder.UseSetting("Authentication:DefaultScheme", "Test");
        }
    }
}

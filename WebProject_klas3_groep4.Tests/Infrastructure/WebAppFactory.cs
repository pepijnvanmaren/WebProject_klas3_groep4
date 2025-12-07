using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebProject_klas3_groep4;
using WebProject_klas3_groep4.Tests;

namespace WebProject_klas3_groep4.Tests.Infrastructure
{
    public class WebAppFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Verwijder de echte database voor de test
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                // Voeg InMemory database toe voor testen
                services.AddDbContext<DatabaseContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDB");
                });

                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        "Test", options => { });

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("TestPolicy", policy =>
                    {
                        policy.AuthenticationSchemes.Add("Test");
                        policy.RequireAuthenticatedUser();
                    });
                });

                // Build provider en seed testdata
                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                    db.Database.EnsureCreated();

                }
            });
        }
    }
}

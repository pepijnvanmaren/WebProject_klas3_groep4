using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using WebProject_klas3_groep4;
using WebProject_klas3_groep4.DTO;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using WebProject_klas3_groep4.Tests.Infrastructure;

public class VeilingSwaggerTest : IClassFixture<WebAppFactory>
{
    private readonly HttpClient _client;

    public VeilingSwaggerTest(WebAppFactory factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // gebruik InMemory DB voor integratietests
                // integratietests checken de connectie en route met Swagger
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<DatabaseContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDB_Veiling");
                });
            });
        }).CreateClient();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Test");
    }

    // -------------------------------
    // TEST 1 — GET ALL VEILINGEN
    [Fact]
    public async Task GetVeilingen_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/Veiling");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var list = await response.Content.ReadFromJsonAsync<List<VeilingOutputDto>>();
        Assert.NotNull(list);
    }


    // -------------------------------
    // TEST 2 — POST VEILING
    [Fact]
    public async Task PostVeiling_CreatesNewVeiling()
    {
        var dto = new VeilingCreateDto
        {
            KlokLocatie = "Zzzzzzzzzzzoetermeer",
            AantalProducten = 10,
            HuidigeSituatieVanVeiling = "Start",
            Bechrijving = "Test"
        };

        var response = await _client.PostAsJsonAsync("/api/Veiling", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<VeilingOutputDto>();
        Assert.Equal("Zzzzzzzzzzzoetermeer", created.KlokLocatie);
    }


    // -------------------------------
    // TEST 3 — DELETE VEILING
    [Fact]
    public async Task DeleteVeiling_RemovesVeiling()
    {
        // maakt iets aan
        var createDto = new VeilingCreateDto
        {
            KlokLocatie = "TestDelete",
            AantalProducten = 5,
            HuidigeSituatieVanVeiling = "Open",
            Bechrijving = "X"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/Veiling", createDto);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<VeilingOutputDto>();

        // en verwijdert hier
        var deleteResponse = await _client.DeleteAsync($"/api/Veiling/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }
}

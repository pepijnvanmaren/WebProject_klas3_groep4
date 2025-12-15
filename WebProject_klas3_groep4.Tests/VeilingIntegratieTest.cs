using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;
using System.Net.Http.Json;
using WebProject_klas3_groep4;
using WebProject_klas3_groep4.DTO;
using WebProject_klas3_groep4.Tests.Infrastructure;
using Xunit;

public class VeilingIntegratieTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public VeilingIntegratieTest(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    // ---------------------------------------------------------------------
    // TEST 1 — POST
    public async Task CreateVeiling_ShouldReturnCreated()
    {
        // Arrange — Test input
        var dto = new VeilingCreateDto
        {
            KlokLocatie = "Zoetermeer",
            AantalProducten = 15,
            HuidigeSituatieVanVeiling = "Open",
            Bechrijving = "Test beschrijving"
        };

        // Act — API aanroepen
        var response = await _client.PostAsJsonAsync("/api/Veiling", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<VeilingOutputDto>();
        Assert.Equal("Zoetermeer", created.KlokLocatie);
    }

    // ---------------------------------------------------------------------
    // TEST 2 — GET all veilingen
    [Fact]
    public async Task GetVeilingen_ShouldReturnOk()
    {
        // ACT
        var response = await _client.GetAsync("/api/Veiling");

        // ASSERT
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ---------------------------------------------------------------------
    // TEST 3 — DELETE werkt
    [Fact]
    public async Task DeleteVeiling_ShouldReturnNoContent()
    {
        // Arrange — maak eerst een item
        var dto = new VeilingCreateDto
        {
            KlokLocatie = "DeleteTest",
            AantalProducten = 5,
            HuidigeSituatieVanVeiling = "Open",
            Bechrijving = "X"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/Veiling", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<VeilingOutputDto>();

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/Veiling/{created.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }
}

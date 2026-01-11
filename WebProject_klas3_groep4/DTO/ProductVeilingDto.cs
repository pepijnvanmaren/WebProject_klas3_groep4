using System.Text.Json.Serialization;

namespace WebProject_klas3_groep4.DTO
{
    public class ProductVeilingDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("naam")]
        public string Naam { get; set; } = null!;

        [JsonPropertyName("foto")]
        public string? Foto { get; set; }

        [JsonPropertyName("beschrijving")]
        public string? Beschrijving { get; set; }

        [JsonPropertyName("hoeveelheid")]
        public int Hoeveelheid { get; set; }

        [JsonPropertyName("minimalePrijs")]
        public int MinimalePrijs { get; set; }
    }
}

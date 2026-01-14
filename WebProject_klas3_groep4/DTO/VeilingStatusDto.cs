using System.Text.Json.Serialization;

namespace WebProject_klas3_groep4.DTO
{
    public class VeilingStatusDto
    {
        [JsonPropertyName("isActief")]
        public bool IsActief { get; set; }

        [JsonPropertyName("isInPauze")]
        public bool IsInPauze { get; set; }

        [JsonPropertyName("remainingSeconds")]
        public int RemainingSeconds { get; set; }

        [JsonPropertyName("huidigProduct")]
        public ProductVeilingDto? HuidigProduct { get; set; }

        [JsonPropertyName("volgendProduct")]
        public ProductVeilingDto? VolgendProduct { get; set; }

        [JsonPropertyName("aantalInWachtrij")]
        public int AantalInWachtrij { get; set; }
    }
    public class ProductDto
    {
        public int Id { get; set; }
        public string Naam { get; set; } = "";
        public string? Foto { get; set; }
        public string? Beschrijving { get; set; }
        public int Hoeveelheid { get; set; }
        public decimal MinimalePrijs { get; set; }
    }
}

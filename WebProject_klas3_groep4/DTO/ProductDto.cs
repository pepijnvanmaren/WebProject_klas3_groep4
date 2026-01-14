namespace WebProject_klas3_groep4.DTO
{
    public class ProductCreateDto
    {
        public string Naam { get; set; } = null!;
        public string? Foto { get; set; } = null;
        public string? Beschrijving { get; set; }
        public DateTime? Oogstdatum { get; set; }
        public int? Potmaat { get; set; }
        public double Gewicht { get; set; }
        public double? Steellengte { get; set; }
        public int Hoeveelheid { get; set; }
        public int MinimalePrijs { get; set; }
        // NIEUW: Veiling koppelen
        public int? VeilingId { get; set; }
    }

    public class ProductUpdateDto
    {
        public string? Naam { get; set; }
        public string? Foto { get; set; }
        public string? Beschrijving { get; set; }
        public DateTime? Oogstdatum { get; set; }
        public int? Potmaat { get; set; }
        public double? Gewicht { get; set; }
        public double? Steellengte { get; set; }
        public int? Hoeveelheid { get; set; }
        public int? MinimalePrijs { get; set; }
        // NIEUW: Veiling wijzigen
        public int? VeilingId { get; set; }
    }

    public class ProductOutputDto
    {
        public int Id { get; set; }
        public string Naam { get; set; } = null!;
        public string? Foto { get; set; }
        public string? Beschrijving { get; set; }
        public DateOnly Oogstdatum { get; set; }
        public int? Potmaat { get; set; }
        public double Gewicht { get; set; }
        public double? Steellengte { get; set; }
        public int Hoeveelheid { get; set; }
        public int MinimalePrijs { get; set; }
        // NIEUW: Info over Aanvoerder en Veiling
        public int? AanvoerderId { get; set; }
        public string? AanvoerderNaam { get; set; }
        public int? VeilingId { get; set; }
        public string? VeilingNaam { get; set; }
        public string? Productstatus { get; set; }
    }
}
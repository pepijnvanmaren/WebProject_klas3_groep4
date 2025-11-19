namespace WebProject_klas3_groep4.models
{
    public class ProductDto
    {
        public string? Naam { get; set; }
        public string? Foto { get; set; }
        public string? Beschrijving { get; set; }

        public DateOnly? Oogstdatum { get; set; }
        public int Potmaat { get; set; }
        public double Gewicht { get; set; }
        public double Steellengte { get; set; }
        public int Hoeveelheid { get; set; }
        public int MinimalePrijs { get; set; }
    }
}
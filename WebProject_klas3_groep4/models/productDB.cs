namespace WebProject_klas3_groep4.models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string? Naam { get; set; }
        public string? Foto { get; set; }
        public string? Beschrijving { get; set; }

        public int AanvoerderID { get; set; }
        public Aanvoerder? Aanvoerder { get; set; }

        public ProductSpecificaties? Specificaties { get; set; }

        public ICollection<Lot>? Lots { get; set; }
    }
}


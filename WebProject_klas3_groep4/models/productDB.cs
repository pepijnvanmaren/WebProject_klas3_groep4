using System.ComponentModel.DataAnnotations;

namespace WebProject_klas3_groep4.models
{
    public class Product
    {
        [Key]
        public int ID { get; set; }
        public string? Naam { get; set; }
        public string? Foto { get; set; }
        public string? Beschrijving { get; set; }
        public List<LotDB> Lists { get; set; }
    }
}


using System.ComponentModel.DataAnnotations;

namespace WebProject_klas3_groep4.models
{
    public class productDB
    {
        [Key]
        public int ID { get; set; }
        public string? Naam { get; set; }
        public string? Foto { get; set; }
        public string? Beschrijving { get; set; }

        public string? bedrijf { get; set; }

        public string? hoeveelheid { get; set; }

        public string? startprijs { get; set; }

        public Boolean gekocht { get; set; } = true;


        public List<LotDB> Lists { get; set; }
    }
}


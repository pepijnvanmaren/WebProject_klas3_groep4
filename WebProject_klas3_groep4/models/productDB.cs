using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject_klas3_groep4.models
{
    public class productDB
    {
        [Key]
        public int ID { get; set; }
        public DateTime? Oogstdatum { get; set; }
        public int? Potmaat { get; set; }
        public double Gewicht { get; set; }
        public double? Steellengte { get; set; }
        public int Hoeveelheid { get; set; }
        public string Naam { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Foto { get; set; }

        public string Beschrijving { get; set; }
        public int MinimalePrijs { get; set; }
    }
}
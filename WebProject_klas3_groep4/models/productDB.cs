using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebProject_klas3_groep4.models
{
    [Index(nameof(AanvoerderId))]
    [Index(nameof(VeilingId))]

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

        public VeilingStatus Status { get; set; } = VeilingStatus.InWachtrij;
        public int? VeilingVolgorde { get; set; }
        public DateTime? VeilingStartTijd { get; set; }
        public DateTime? VerkochtOp { get; set; }
        public int? KoperID { get; set; }
        public double? VerkochtePrijs { get; set; }
        public bool IsGekocht { get; set; } = false;

       

        // ===== NIEUW: Relatie naar Aanvoerder (GebruikerDB) =====
        [ForeignKey("Aanvoerder")]
        public int? AanvoerderId { get; set; }
        public GebruikerDB? Aanvoerder { get; set; }

        // ===== NIEUW: Relatie naar Veiling =====
        [ForeignKey("Veiling")]
        public int? VeilingId { get; set; }
        public VeilingDB? Veiling { get; set; }


    } 
    public enum VeilingStatus
        {
            InWachtrij = 0,      // Product wacht om geveild te worden
            Actief = 1,          // Product is nu actief in de veiling
            Verkocht = 2,        // Product is verkocht (na pauze)
            Geannuleerd = 3,     // Product is uit veiling gehaald
            VerlatenVeiling = 4  // Veiling gestopt zonder verkoop
        }
}

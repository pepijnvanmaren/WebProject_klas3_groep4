using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebProject_klas3_groep4.models;
namespace WebProject_klas3_groep4
{
    [Index(nameof(VeilingmeesterId))]
    public class VeilingDB
    {
        [Key]
        public int ID { get; set; }

        public String StarTijd { get; set; }
        public String StartDatum { get; set; }
        public int AantalProducten { get; set; }
        public String KlokLocatie { get; set; }
        public String HuidigeSituatieVanVeiling { get; set; }
        public String Bechrijving { get; set; }

        // ===== NIEUW: Relatie naar Veilingmeester =====
        [ForeignKey("Veilingmeester")]
        public int? VeilingmeesterId { get; set; }
        public GebruikerDB? Veilingmeester { get; set; }

        // ===== NIEUW: Relatie naar Producten =====
        public List<productDB>? Producten { get; set; } = new List<productDB>();
    }
}
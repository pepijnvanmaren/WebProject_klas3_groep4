using System.ComponentModel.DataAnnotations;
using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4
{
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
        public List<productDB> Lists { get; set; }

    }
}

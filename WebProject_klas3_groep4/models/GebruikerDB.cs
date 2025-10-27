using System.ComponentModel.DataAnnotations;

namespace WebProject_klas3_groep4.models
{
    public class GebruikerDB
    {
        [Key]
        public int ID { get; set; }
        public string Naam { get; set; }  
        public int Telefoonnummer { get; set; }
        public string Email { get; set; }
        public string Rol {  get; set; }
        public string Paswoord { get; set; }


    }
}


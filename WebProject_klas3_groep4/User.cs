using Microsoft.AspNetCore.Identity;

namespace WebProject_klas3_groep4.Models
{
    public class User : IdentityUser
    {
        public string Naam { get; set; }
        public int Telefoonnummer { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
        public string Paswoord { get; set; }
        public string EmailBevestigd { get; set; }
        public string TelefoonnummerBevestigd { get; set; } 

    }
}
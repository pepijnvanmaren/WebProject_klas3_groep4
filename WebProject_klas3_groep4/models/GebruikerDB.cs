using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
namespace WebProject_klas3_groep4.models
{
    public class GebruikerDB : IdentityUser<int>
    {
        // Identity heeft al: Id, UserName, Email, PhoneNumber, PasswordHash
        // Discriminator voor rol type
        public string Rol { get; set; } = "Gebruiker"; // "Koper", "Aanvoerder", "Veilingmeester"
        // Koper properties
        public string? BankGegevens { get; set; }
        public string? Postcode { get; set; }
        public string? Adres { get; set; }
        // Aanvoerder properties
        public string? KvkNummer { get; set; }
        public string? NaamVanBedrijf { get; set; }
        public string? BedrijfTelefoonnummer { get; set; }
        public string? BedrijfEmail { get; set; }
        public List<productDB>? Producten { get; set; } = new List<productDB>();
        // Veilingmeester properties
        public string? VeilingVestiging { get; set; }
        public List<VeilingDB>? Veilingen { get; set; } = new List<VeilingDB>();
    }
}
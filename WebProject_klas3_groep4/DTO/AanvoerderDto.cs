using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4.DTO
{
    public class AanvoerderDto : GebruikerDto
    {
        public string KvkNummer { get; set; }

        public string NaamVanBedrijf { get; set; }

        public string Postcode { get; set; }

        public string Adres { get; set; }

        public string BedrijfTelefoonnummer { get; set; }

        public string BedrijfEmail { get; set; }
        public List<productDB> prodcten { get; set; }
    }

    public class AanvoerderDataDto 
    {
        public string Naam { get; set; }
        public string Email { get; set; }
        public int Telefoonnummer { get; set; }
    }

    public class AanvoerderProductenDto
    {
        public List<productDB> prodcten { get; set; }
    }
}

namespace WebProject_klas3_groep4.DTO
{
    public class AanvoerderOutputDto
    {

        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string? KvkNummer { get; set; }
        public string? NaamVanBedrijf { get; set; }
        public string? Postcode { get; set; }
        public string? Adres { get; set; }
        public string? BedrijfTelefoonnummer { get; set; }
        public string? BedrijfEmail { get; set; }
    }

    public class AanvoerderCreateDto
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Rol { get; set; } = "Koper";
        public string? KvkNummer { get; set; }
        public string? NaamVanBedrijf { get; set; }
        public string? Postcode { get; set; }
        public string? Adres { get; set; }
        public string? BedrijfTelefoonnummer { get; set; }
        public string? BedrijfEmail { get; set; }
    }

    public class AanvoerderUpdateDto
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? KvkNummer { get; set; }
        public string? NaamVanBedrijf { get; set; }
        public string? Postcode { get; set; }
        public string? Adres { get; set; }
        public string? BedrijfTelefoonnummer { get; set; }
        public string? BedrijfEmail { get; set; }
    }
}
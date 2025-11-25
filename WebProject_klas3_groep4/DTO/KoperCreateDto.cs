namespace WebProject_klas3_groep4.DTO
{
    public class KoperCreateDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string? BankGegevens { get; set; }
        public string? Adres { get; set; }
        public string? Postcode { get; set; }
    }

    public class KoperUpdateDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? BankGegevens { get; set; }
        public string? Adres { get; set; }
        public string? Postcode { get; set; }
    }

    public class KoperOutputDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Rol { get; set; } = "Koper";
        public string? BankGegevens { get; set; }
        public string? Adres { get; set; }
        public string? Postcode { get; set; }
    }
}
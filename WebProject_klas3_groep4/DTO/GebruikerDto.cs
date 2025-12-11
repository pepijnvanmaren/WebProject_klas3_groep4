namespace WebProject_klas3_groep4.DTO
{
    public class GebruikerDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Rol { get; set; } = null!;
    }

    public class GebruikerCreateDto
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Rol { get; set; }
    }

    public class GebruikerUpdateDto
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? NewPassword { get; set; }
    }
    public class UpdatePasswordDto
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
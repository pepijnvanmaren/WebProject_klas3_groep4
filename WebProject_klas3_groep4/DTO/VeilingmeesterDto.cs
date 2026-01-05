namespace WebProject_klas3_groep4.DTO
{
    public class VeilingmeesterCreateDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string? VeilingVestiging { get; set; }
    }

    public class VeilingmeesterUpdateDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? VeilingVestiging { get; set; }
    }

    public class VeilingmeesterOutputDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? VeilingVestiging { get; set; }
        public string Rol { get; set; } = "Veilingmeester";
        // Veilingen die deze veilingmeester beheert
        public List<int>? VeilingIds { get; set; }
    }
}
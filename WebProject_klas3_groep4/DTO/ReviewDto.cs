namespace WebProject_klas3_groep4.DTO
{
    public class Review
    {
        public int Id { get; set; }
        public GebruikerDto Gebruiker { get; set; } = null!;
        public string Tekst { get; set; }
        public int Sterren { get; set; }
    }
}

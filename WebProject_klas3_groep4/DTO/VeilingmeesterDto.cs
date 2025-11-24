namespace WebProject_klas3_groep4.DTO
{
    public class VeilingmeesterDto : GebruikerDto
    {
        public String VeilingVestiging { get; set; }
        public List<VeilingDB> Veilingen { get; set; }
    }
}

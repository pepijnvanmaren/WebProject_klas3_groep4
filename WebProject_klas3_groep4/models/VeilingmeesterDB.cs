using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4
{
    public class VeilingmeesterDB : GebruikerDB
    {
        public String VeilingVestiging { get; set; }
        public List<VeilingDB> Veilingen { get; set; }
    }
}

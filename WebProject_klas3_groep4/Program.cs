
namespace WebProject_klas3_groep4
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MyContext c = new MyContext();
            c.Veilingmeesters.Add(new VeilingmeesterDB() {Naam = "Haijin", Telefoonnummer = 0675728539, Email = "Haijin@gmail.com", Rol = "Veilingmeester", VeilingVestiging = "Den Haag" });
            c.SaveChanges();
            c.Veilingmeesters.Single((s) => s.ID == 1)
                .Veilingen.Add(new VeilingDB() { ID = 1, AantalProducten = 10, Bechrijving = "Dingen van mensen verkopen", HuidigeSituatieVanVeiling = "Nog niet begonnen", KlokLocatie = "Den Haag", StartDatum = "12-02-2030", StarTijd = "21:20" });
        }
    }
}

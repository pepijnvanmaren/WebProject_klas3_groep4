namespace WebProject_klas3_groep4.DTO
{
    public class VeilingCreateDto
    {
        public string? StarTijd { get; set; }
        public string? StartDatum { get; set; }
        public int AantalProducten { get; set; }
        public string? KlokLocatie { get; set; }
        public string? HuidigeSituatieVanVeiling { get; set; }
        public string? Bechrijving { get; set; }
        // NIEUW: Veilingmeester koppelen
        public int? VeilingmeesterId { get; set; }
    }

    public class VeilingUpdateDto
    {
        public string? StarTijd { get; set; }
        public string? StartDatum { get; set; }
        public int? AantalProducten { get; set; }
        public string? KlokLocatie { get; set; }
        public string? HuidigeSituatieVanVeiling { get; set; }
        public string? Bechrijving { get; set; }
        // NIEUW: Veilingmeester wijzigen
        public int? VeilingmeesterId { get; set; }
    }

    public class VeilingOutputDto
    {
        public int Id { get; set; }
        public string? StarTijd { get; set; }
        public string? StartDatum { get; set; }
        public int AantalProducten { get; set; }
        public string? KlokLocatie { get; set; }
        public string? HuidigeSituatieVanVeiling { get; set; }
        public string? Bechrijving { get; set; }
        // NIEUW: Veilingmeester info
        public int? VeilingmeesterId { get; set; }
        public string? VeilingmeesterNaam { get; set; }
        // NIEUW: Producten in deze veiling
        public List<ProductOutputDto>? Producten { get; set; }
    }
}
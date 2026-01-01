
namespace WebProject_klas3_groep4.DTO
{
    public class VerkochteProductenCreateDto
    {
        public int HoeveelHeid { get; set; }
        public double VerkochtePrijs { get; set; }
        public int ProductId { get; set; }
        public int KoperId { get; set; }
        public DateTime VerkoopDatum { get; set; }
    }

    public class VerkochteProductenOutputDto
    {
        public int ID { get; set; }
        public int HoeveelHeid { get; set; }
        public double VerkochtePrijs { get; set; }
        public int ProductId { get; set; }
        public int KoperId { get; set; }
        public DateTime VerkoopDatum { get; set; }
    }

    public class GemiddeldeAllesDto
    {
        public double VerkochtePrijs { get; set; }
        public int HoeveelHeid { get; set; }
    }
}

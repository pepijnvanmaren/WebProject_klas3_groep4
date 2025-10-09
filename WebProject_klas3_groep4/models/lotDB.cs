
namespace WebProject_klas3_groep4.models
{
    public class LotDB
    {
        public class Lot
        {
            public int LotID { get; set; }
            public int ProductID { get; set; }
            public Product? Product { get; set; }

            public int VeilingID { get; set; }
            public Veiling? Veiling { get; set; }

            public DateTime? GebeurtenisDatum { get; set; }
            public string? GewensteLocatie { get; set; }

            public ICollection<Bod>? Biedingen { get; set; }
        }
    }
}


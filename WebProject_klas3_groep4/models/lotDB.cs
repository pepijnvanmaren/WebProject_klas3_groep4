
using System.ComponentModel.DataAnnotations;

namespace WebProject_klas3_groep4.models
{
    public class LotDB
    {
        [Key]
        public int ID { get; set; }
            public int ProductID { get; set; }
            public int VeilingID { get; set; }
            public DateTime? GebeurtenisDatum { get; set; }
            public string? GewensteLocatie { get; set; }

        
    }
}


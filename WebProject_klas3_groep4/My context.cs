using Microsoft.EntityFrameworkCore;


namespace WebProject_klas3_groep4
{
    public class MyContext : DbContext​
    
    {​
        public DbSet<VeilingmeesterDB>Veilingmeesteers { get; set; }
        public DbSet<VeilingDB>Veilingen {  get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder b) => b.UseSqlite("Data Source=database.db");​
        public DbSet<VeilingmeesterDB> Veilingmeesters { get; set; }​ 
    }

}
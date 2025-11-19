using Microsoft.EntityFrameworkCore;
using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<VeilingmeesterDB> Veilingmeesters { get; set; }
        public DbSet<GebruikerDB> Gebruikers { get; set; }
        public DbSet<VeilingDB> Veilingen { get; set; }

        public DbSet<AanvoerderDB> Aanvoerder { get; set; }

        public DbSet<KoperDB> Koper { get; set; }
        public DbSet<productDB> product { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=CRAPTOP\\SQLEXPRESS01;Database=WebProject_klas3_groep4;Trusted_Connection=True;");
            }
        }

       
    }
}
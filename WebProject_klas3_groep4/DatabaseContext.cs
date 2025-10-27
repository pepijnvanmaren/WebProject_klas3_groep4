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

        public DbSet<LotDB> Lots { get; set; }
        public DbSet<AanvoerderDB> Aanvoerder { get; set; }

        public DbSet<KoperDB> Koper { get; set; }
        public DbSet<productDB> product { get; set; }






        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=database.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GebruikerDB>().HasData(
                new GebruikerDB
                {
                    ID = 1,
                    Naam = "Admin",
                    Telefoonnummer = 123456789,
                    Email = "admin@example.com",
                    Rol = "Administrator"
                }
            );

            // Voeg hier meer seed-data toe voor andere modellen
        }
    }
}
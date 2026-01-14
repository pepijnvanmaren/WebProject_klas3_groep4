using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using WebProject_klas3_groep4.DTO;
using WebProject_klas3_groep4.models;

namespace WebProject_klas3_groep4
{
    public class DatabaseContext : IdentityDbContext<GebruikerDB, IdentityRole<int>, int>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        // Alle gebruikers in één tabel (Users wordt al door Identity gedefinieerd)
        public DbSet<GebruikerDB> Gebruikers => Users;

        // Andere tabellen
        public DbSet<VeilingDB> Veilingen { get; set; }
        public DbSet<productDB> Producten { get; set; }
        public DbSet<VerkochteProdcutenDB> VerkochteProducten { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=CRAPTOP\\SQLEXPRESS01;Database=WebProject_klas3_groep4;Trusted_Connection=True;");
            }
        }
        public DbSet<GemiddeldeAllesDto> GemiddeldeAlles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configureer relaties
            builder.Entity<GebruikerDB>()
                .HasMany(g => g.Producten)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<GebruikerDB>()
                .HasMany(g => g.Veilingen)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<VerkochteProdcutenDB>()
                .HasOne(v => v.Product)
                .WithMany()
                .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<VerkochteProdcutenDB>()
                .HasOne(v => v.Koper)
                .WithMany()
                .HasForeignKey(v => v.KoperId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<GemiddeldeAllesDto>().HasNoKey();

        }
    }
}
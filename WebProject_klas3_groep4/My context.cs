using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace WebProject_klas3_groep4
{
    public class MyContext : DbContext​
    {​

 protected override void OnConfiguring(DbContextOptionsBuilder b) => b.UseSqlite("Data Source=database.db");​
 // b.UseSqlServer("Connection string") //Je kan de keuze van de database ook in de startup van de app plaatsen

 protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Studenten { get; set; }​ 
}

}
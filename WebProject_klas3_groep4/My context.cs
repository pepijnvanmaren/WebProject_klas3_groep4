using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace WebProject_klas3_groep4
{
    public class MyContext : DbContext​
    {​
        protected override void OnConfiguring(DbContextOptionsBuilder b) => b.UseSqlite("Data Source=database.db");​
        public DbSet<VeilingmeesterDB> Veilingmeesters { get; set; }​ 
}

}
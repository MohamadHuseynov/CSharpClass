using Microsoft.EntityFrameworkCore;
using Model.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class FinalProjectDbContext : DbContext
    {
        public FinalProjectDbContext(DbContextOptions options) : base(options)
        {

        }

        public FinalProjectDbContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-E5H9JT6;Initial Catalog=FinalProjectDb;Integrated Security=True;MultipleActiveResultSets=true;Encrypt=False;");
        }
        public DbSet<Person> Person { get; set; }

        public DbSet<Product> Product { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using Model.DomainModels;

namespace Model
{
    public class FinalProjectDbContext : DbContext
    {
        public FinalProjectDbContext(DbContextOptions<FinalProjectDbContext> options) : base(options)
        {
        }

        public FinalProjectDbContext() : base()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=MOHAMMAD;Initial Catalog=FinalProjectDb;Integrated Security=True;MultipleActiveResultSets=true;Encrypt=False;");
            }
        }

        public DbSet<Person> Person { get; set; }
        public DbSet<Product> Product { get; set; }


    }
}
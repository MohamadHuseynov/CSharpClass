using Microsoft.EntityFrameworkCore;
using Model.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class FinalProjectDbContext:DbContext
    {
        public FinalProjectDbContext(DbContextOptions options):base(options)
        {
            
        }
        public DbSet<Person> Person { get; set; }

        public DbSet<Product> Product { get; set; }
    }
}

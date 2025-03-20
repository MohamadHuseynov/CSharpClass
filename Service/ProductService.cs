using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Model;
using Model.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Service
{
    public class ProductService
    {
        public void AddProduct(Product product)
        {
            using (var context = new FinalProjectDbContext())
            {
                context.Product.Add(product);
                context.SaveChanges();

            }
        }

        //public void TestDataBaseConnect()
        //{
        //    using (var context = new FinalProjectDbContext())
        //    {
        //        if (context.Database.Exists())
        //        {
        //            Console.WriteLine("Database connection successful");
        //        }
        //        else
        //        {
        //            Console.WriteLine("Database connection failed");
        //        }
        //    }
        //}
    }
}

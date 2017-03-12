using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using MVCapp.Models;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace MVCapp.DAL
{
    public class OnlineShopDataContext : DbContext
    {
        public OnlineShopDataContext() : base("OnlineDataShopContext")
        { }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ShoppingCart> ShoppingCart { get; set; }

        public DbSet<RegionCoordinates> RegionCoordinates { get; set; } 

        public DbSet<Dummy> Dummy { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
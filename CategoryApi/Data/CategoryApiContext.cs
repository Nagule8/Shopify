using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration.Conventions;
using CategoryApi.Models;

namespace CategoryApi.Data
{
    public class CategoryApiContext : DbContext
    {
        public CategoryApiContext() : base("CategoryApiContext")
        {
            this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        public DbSet<Category> Categories { get; set; }
        public System.Data.Entity.DbSet<CategoryApi.Models.Item> Items { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public System.Data.Entity.DbSet<CategoryApi.Models.CartItem> CartItems { get; set; }

        public System.Data.Entity.DbSet<CategoryApi.Models.User> Users { get; set; }

        public System.Data.Entity.DbSet<CategoryApi.Models.RegisterUser> RegisterUsers { get; set; }
    }
}

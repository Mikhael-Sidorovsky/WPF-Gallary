using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallery.Models
{
    class GallaryContext : DbContext
    {
        static GallaryContext()
        {
            Database.SetInitializer<GallaryContext>(new DbInitializer());
        }
        public GallaryContext() : base("GallaryDB") { }

        public DbSet<User> Users { get; set; }
        public DbSet<MyImage> Images { get; set; }
        public DbSet<Mark> Marks { get; set; }
    }
}

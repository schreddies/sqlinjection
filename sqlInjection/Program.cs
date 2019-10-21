using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SqlInjection
{
    class Program
    {
        static async Task Main(string[] args)
        {
            CreateAndSeedDatabase();

            //// 1
            //using (var db = new BloggingContext())
            //{
            //    var author = "John Smith";
            //    //var query = "Select * from Blogs WHERE Author = " + author;
            //    var blogs = await db.Blogs.FromSql("Select * from Blogs WHERE Author = " + author).ToListAsync();
            //}

            //// 2
            //using (var db = new BloggingContext())
            //{
            //    var author = "John Smith";
            //    var query = $@"Select * from Blogs WHERE Author={author}";
            //    var blogs = await db.Blogs.FromSql(query).ToListAsync();
            //}

            //// 3
            //using (var db = new BloggingContext())
            //{
            //    var author = "John Smith";
            //    var blogs = await db.Blogs.FromSql(@"Select * from Blogs WHERE Author = {0}", author).ToListAsync();
            //}


            // 4
            using (var db = new BloggingContext())
            {
                var author = "John Smith";
                FormattableString query = $@"Select * from Blogs WHERE Author={author}";
                var blogs = await db.Blogs.FromSql(query).ToListAsync();
            }

            Console.ReadKey();
        }

        static void CreateAndSeedDatabase()
        {
            using (var db = new BloggingContext())
            {
                if (db.Database.EnsureCreated())
                {
                    SeedData(db);
                }
            }
        }


        static void SeedData(BloggingContext db)
        {
            JObject o = JObject.Parse(@"{ Type:'Warning', Data:'Not for everyone'}");
            db.Blogs.Add(new Blog { Title = "I love dogs", Author = "John Smith" });
            db.Blogs.Add(new Blog { Title = "I love cats too", Author = "Jane Smith" });

            db.SaveChanges();
        }
    }

    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=SQLInjection;Trusted_Connection=True;")
                .UseLoggerFactory(new LoggerFactory().AddConsole());
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
    }
}
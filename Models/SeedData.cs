using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using FirstScent.Data;

namespace FirstScent.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new FirstScentContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<FirstScentContext>>()))
            {
                // Look for any movies.
                if (context.Fragrances.Any())
                {
                    return;   // DB has been seeded
                }
                //context.Movies.AddRange(
                //    new Fragrance
                //    {
                //        Name = "When Harry Met Sally",
                //        Category = "Romantic Comedy",
                //        Price = 7.99M,
                //        Rating = "R"
                //    },
                //    new Fragrance
                //    {
                //        Title = "Ghostbusters ",
                //        ReleaseDate = DateTime.Parse("1984-3-13"),
                //        Genre = "Comedy",
                //        Price = 8.99M,
                //        Rating = "B"

                //    },
                //    new Fragrance
                //    {
                //        Title = "Ghostbusters 2",
                //        ReleaseDate = DateTime.Parse("1986-2-23"),
                //        Genre = "Comedy",
                //        Price = 9.99M, 
                //        Rating = "B"
                //    },
                //    new Fragrance
                //    {
                //        Title = "Rio Bravo",
                //        ReleaseDate = DateTime.Parse("1959-4-15"),
                //        Genre = "Western",
                //        Price = 3.99M, 
                //        Rating = "C"
                //    }
                //);
                context.SaveChanges();
            }
        }
    }
}

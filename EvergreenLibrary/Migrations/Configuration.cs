namespace EvergreenLibrary.Migrations
{
    using EvergreenLibrary.Infrastructure;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<EvergreenLibrary.Infrastructure.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(EvergreenLibrary.Infrastructure.ApplicationDbContext context)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

            var user = new ApplicationUser()
            {
                UserName = "natalia.lytovchenko@yopmail.com",
                Email = "natalia.lytovchenko@yopmail.com",
                EmailConfirmed = true,
                FirstName = "Natalia",
                LastName = "Lytovchenko"
            };
            var user1 = new ApplicationUser()
            {
                UserName = "olga.makarova@yopmail.com",
                Email = "olga.makarova@yopmail.com",
                EmailConfirmed = true,
                FirstName = "Olga",
                LastName = "Makarova"
            };
            var user2 = new ApplicationUser()
            {
                UserName = "anastasiya.shapoval@yopmail.com",
                Email = "anastasiya.shapoval@yopmail.com",
                EmailConfirmed = true,
                FirstName = "Anastasiya",
                LastName = "Shapoval"
            };

            manager.Create(user, "Qwerty12345");
            manager.Create(user1, "Qwerty12345");
            manager.Create(user2, "Qwerty12345");

            if (roleManager.Roles.Count() == 0)
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
                roleManager.Create(new IdentityRole { Name = "Librarian" });
                roleManager.Create(new IdentityRole { Name = "Customer" });
            }

            var adminUser = manager.FindByName("natalia.lytovchenko@yopmail.com");

            manager.AddToRoles(adminUser.Id, new string[] { "Admin" });

            var librarianUser = manager.FindByName("olga.makarova@yopmail.com");

            manager.AddToRoles(librarianUser.Id, new string[] { "Librarian" });

            var customerUser = manager.FindByName("anastasiya.shapoval@yopmail.com");

            manager.AddToRoles(customerUser.Id, new string[] { "Customer" });

            context.Books.AddOrUpdate( x => x.Id,
                new Book()
                {
                    Title = "Pride and Prejudice",
                    Year = 1813,
                    Author = "Jane Austen",
                    NeedToDelete = false,
                    ApplicationUserId = customerUser.Id
                },
                new Book()
                {
                    Title = "Northanger Abbey",
                    Year = 1817,
                    Author = "Jane Austen",
                    NeedToDelete = false
                },
                new Book()
                {
                    Title = "David Copperfield",
                    Year = 1850,
                    Author = "Charles Dickens",
                    NeedToDelete = false
                },
                new Book()
                {
                    Title = "Don Quixote",
                    Year = 1617,
                    Author = "Miguel de Cervantes",
                    NeedToDelete = false
                }
                );
        }
    }
}

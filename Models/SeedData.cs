//PASUL 4 CREARE SEED

using MusicMatch.Data;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicMatch.Data;
using MusicMatch.Models;
using System.Collections.Generic;

namespace MusicMatch.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider
        serviceProvider)
        {
            using (var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService
            <DbContextOptions<ApplicationDbContext>>()))
            {
                // Verificam daca in baza de date exista cel putin un rol insemnand ca a fost rulat codul
                // De aceea facem return pentru a nu insera rolurile inca o data
                // Acesta metoda trebuie sa se execute o singura data
                if (context.Roles.Any())
                {
                    return; // baza de date contine deja roluri
                }

                // CREAREA ROLURILOR IN BD
                // daca nu contine roluri, acestea se vor crea
                context.Roles.AddRange(
                    new IdentityRole
                    {
                        Id = "2c5e174e-3b0e-446f-86af-483d56fd7210",
                        Name = "Admin",
                        NormalizedName = "Admin".ToUpper()
                    },
                    new IdentityRole
                    {
                        Id = "2c5e174e-3b0e-446f-86af-483d56fd7211",
                        Name = "Editor",
                        NormalizedName = "Editor".ToUpper()
                    },
                    new IdentityRole
                    {
                        Id = "2c5e174e-3b0e-446f-86af-483d56fd7212",
                        Name = "User",
                        NormalizedName = "User".ToUpper()
                    }
                    );


                // o noua instanta pe care o vom utiliza pentru crearea parolelor utilizatorilor
                // parolele sunt de tip hash

                var hasher = new PasswordHasher<ApplicationUser>();
                //ADMIN b0 - admin 10
                //EDITOR b3 - editor 11 
                //USER b4 - user 12

                // CREAREA USERILOR IN BD
                // Se creeaza cate un user pentru fiecare rol
                context.Users.AddRange(
                new ApplicationUser
                {
                    // primary key
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb0",

                    UserName = "admin@proiect.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "ADMIN@PROIECT.COM",
                    Email = "admin@proiect.com",
                    NormalizedUserName = "ADMIN@PROIECT.COM",
                    PasswordHash = hasher.HashPassword(null, "Admin1!"),
                    FirstName = "Admin",
                    LastName = "Admin"
                },

                new ApplicationUser
                {
                    // primary key
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb3",

                    UserName = "editor@proiect.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "EDITOR@PROIECT.COM",
                    Email = "editor@proiect.com",
                    NormalizedUserName = "EDITOR@PROIECT.COM",
                    PasswordHash = hasher.HashPassword(null, "Editor1!"),
                    FirstName = "Editor",
                    LastName = "Edtr"
                },

                new ApplicationUser
                {
                    // primary key
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb4",

                    UserName = "user@proiect.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "USER@PROIECT.COM",
                    Email = "user@proiect.com",
                    NormalizedUserName = "USER@PROIECT.COM",
                    PasswordHash = hasher.HashPassword(null, "User1!"),
                    FirstName = "User",
                    LastName = "User"
                }
             );

                // ASOCIEREA USER-ROLE
                context.UserRoles.AddRange(
                new IdentityUserRole<string>
                {

                    RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210", //rol de admin

                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb0" //admin
                },
                new IdentityUserRole<string>
                {

                    RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7211", //rol de editor

                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb3" //editor
                },

                new IdentityUserRole<string>
                {

                    RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7212", //rol de user

                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb4" //user
                }
                );
                context.SaveChanges();
            }
        }
    }
}

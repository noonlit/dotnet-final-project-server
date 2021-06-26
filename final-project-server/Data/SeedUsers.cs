using FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Data
{
	public class SeedUsers
	{
        private static string Characters = "abcdefghijklmnopqrstuvwxyz123456890";
        private static Random random = new Random();
        public static void Seed(ApplicationDbContext context, UserManager<ApplicationUser> userManager, int count)
        {

            context.Database.EnsureCreated();

            for (int i = 0; i < count; ++i)
            {
                var email = generateRandomString(3, 10) + "@" + generateRandomString(2, 3);
                var user = new ApplicationUser
                {

                    Email = email,
                    UserName = email,
                };

                user.PasswordHash = userManager.PasswordHasher.HashPassword(user, "P@ssw0rd1!");
                context.ApplicationUsers.Add(user);
                context.SaveChanges();
            }
        }

        private static string generateRandomString(int min, int max)
        {
            string s = "";

            for (int j = 0; j < random.Next(min, max); ++j)
            {
                s += Characters[random.Next(Characters.Length)];
            }

            return s;
        }
    }
}

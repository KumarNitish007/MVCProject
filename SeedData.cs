
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MVCProject.Models;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace MVCProject
{
	public class SeedData
	{
		public static async Task Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			// Ensure the database is created
			var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
			context.Database.EnsureCreated();

			// Seed Roles
			if (!roleManager.Roles.Any())
			{
				await roleManager.CreateAsync(new IdentityRole("Admin"));
				await roleManager.CreateAsync(new IdentityRole("User"));
			}

			// Seed default user
			if (!userManager.Users.Any())
			{
				var user = new ApplicationUser { UserName = "admin@example.com", Email = "admin@example.com" };
				var result = await userManager.CreateAsync(user, "Password123!");

				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(user, "Admin");
				}
			}
		}
	}
}

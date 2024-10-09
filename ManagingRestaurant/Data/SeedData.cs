using ManagingRestaurant.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;

namespace ManagingRestaurant.Data
{
    public class SeedData
    {
        private RestaurantContext _context;

        public SeedData(RestaurantContext context)
        {
            _context = context;
        }

        public async Task SeedRole()
        {
            var roleStore = new RoleStore<IdentityRole>(_context);
            if (!_context.Roles.Any(r => r.Name == RoleName.Administrator))
            {
                await roleStore.CreateAsync(new IdentityRole { Name = RoleName.Administrator, NormalizedName = RoleName.Administrator.ToUpper() });
            }
            if (!_context.Roles.Any(r => r.Name == RoleName.User))
            {
                await roleStore.CreateAsync(new IdentityRole { Name = RoleName.User, NormalizedName = RoleName.User.ToUpper() });
            }
            if (!_context.Roles.Any(r => r.Name == RoleName.Manager))
            {
                await roleStore.CreateAsync(new IdentityRole { Name = RoleName.Manager, NormalizedName = RoleName.Manager.ToUpper() });
            }

        }
        public async Task SeedUser()
        {
            var admin = new AppUser()
            {
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                EmailConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                Created_at = DateTime.Now,
                Update_by = "Admin",
                Created_by = "Admin",
                Address = "HN",
                Status = 1,
                PhoneNumber = "+111111111111",
                Update_at = DateTime.Now,
                Image = "https://i.pinimg.com/originals/2a/44/dd/2a44ddddb03ab7f755db5c5c4379404e.png",
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var user = new AppUser()
            {
                UserName = "User",
                NormalizedUserName = "USER",
                Email = "user@gmail.com",
                NormalizedEmail = "USER@GMAIL.COM",
                EmailConfirmed = true,
                TwoFactorEnabled = true,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString(),
                Created_at = DateTime.Now,
                Update_by = "Admin",
                Created_by = "Admin",
                Address = "HN",
                Status = 1,
                PhoneNumber = "+111111111111",
                Update_at = DateTime.Now,
                Image = "https://www.pngkey.com/png/full/72-729716_user-avatar-png-graphic-free-download-icon.png",
            };

            var manager = new AppUser()
            {
                UserName = "Manager",
                NormalizedUserName = "MANAGER",
                Email = "manager@gmail.com",
                NormalizedEmail = "MANAGER@GMAIL.COM",
                EmailConfirmed = true,
                LockoutEnabled = false,
                TwoFactorEnabled = true,
                Created_at = DateTime.Now,
                SecurityStamp = Guid.NewGuid().ToString(),
                Update_by = "Admin",
                Created_by = "Admin",
                Address = "HN",
                Status = 1,
                PhoneNumber = "+111111111111",
                Update_at = DateTime.Now,
                Image = "https://www.pngkey.com/png/full/72-729716_user-avatar-png-graphic-free-download-icon.png",
            };

            var roleStore = new RoleStore<IdentityRole>(_context);
            var roleAdmin = await roleStore.FindByNameAsync(RoleName.Administrator);
            var roleUser = await roleStore.FindByNameAsync(RoleName.User);
            var roleManager = await roleStore.FindByNameAsync(RoleName.Manager);

            if (!_context.Users.Any(u => u.UserName == admin.UserName))
            {
                var passwordAdmin = new PasswordHasher<AppUser>();
                var hashed = passwordAdmin.HashPassword(admin, "admin123");
                admin.PasswordHash = hashed;
                var userStore = new UserStore<AppUser>(_context);
                await userStore.CreateAsync(admin);
                await userStore.AddToRoleAsync(admin, roleAdmin.Name);
            }

            if (!_context.Users.Any(u => u.UserName == user.UserName))
            {
                var passwordUser = new PasswordHasher<AppUser>();
                var hashed = passwordUser.HashPassword(user, "user123");
                user.PasswordHash = hashed;
                var userStore = new UserStore<AppUser>(_context);
                await userStore.CreateAsync(user);
                await userStore.AddToRoleAsync(user, roleUser.Name);
            }

            if (!_context.Users.Any(u => u.UserName == manager.UserName))
            {
                var passwordManager = new PasswordHasher<AppUser>();
                var hashed = passwordManager.HashPassword(manager, "manager123");
                manager.PasswordHash = hashed;
                var userStore = new UserStore<AppUser>(_context);
                await userStore.CreateAsync(manager);
                await userStore.AddToRoleAsync(manager, roleManager.Name);
            }

            await _context.SaveChangesAsync();
        }
    }
}

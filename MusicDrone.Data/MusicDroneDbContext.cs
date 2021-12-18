using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicDrone.Data.Constants;
using MusicDrone.Data.Models;
using System;

namespace MusicDrone.Data
{
    public class MusicDroneDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomsUsers> RoomsUsers { get; set; }

        public MusicDroneDbContext(DbContextOptions<MusicDroneDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var adminRoleId = new Guid("bc963479-34f3-42b7-8d32-41bae9c47742");
            var adminId = new Guid("cdd4f090-d8aa-4c14-9cd5-fe3464ff3bbb");

            modelBuilder.Entity<ApplicationRole>().HasData(new ApplicationRole { Id = new Guid("85b19000-bc92-435a-8d6b-ea780217d030"), Name = Constants.Roles.USERS, NormalizedName = Constants.Roles.USERS.ToUpper() });
            modelBuilder.Entity<ApplicationRole>().HasData(new ApplicationRole { Id = new Guid("e3e7bd47-14c6-4a22-81c7-9381d1b4db70"), Name = Constants.Roles.PREMIUMUSERS, NormalizedName = Constants.Roles.PREMIUMUSERS.ToUpper() });
            modelBuilder.Entity<ApplicationRole>().HasData(new ApplicationRole { Id = new Guid("7d0900cc-5def-4708-b88c-9dd66c7db1ef"), Name = Constants.Roles.MODERATORS, NormalizedName = Constants.Roles.MODERATORS.ToUpper() });
            modelBuilder.Entity<ApplicationRole>().HasData(new ApplicationRole { Id = adminRoleId, Name = Constants.Roles.ADMINISTRATORS, NormalizedName = Constants.Roles.ADMINISTRATORS.ToUpper() });


            //a hasher to hash the password before seeding the user to the db
            var hasher = new PasswordHasher<IdentityUser>();
            
            var username = "admin@music.drone";
            var email = "admin@music.drone";
            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = adminId,
                    UserName = username,
                    NormalizedUserName = username.ToUpper(),
                    Email = email,
                    NormalizedEmail = email.ToUpper(),
                    FirstName = "AdminName",
                    LastName = "AdminLastName",
                    PasswordHash = hasher.HashPassword(null, AuthorizationConstants.DEFAULT_PASSWORD),
                    SecurityStamp = adminId.ToString()
                });

            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(
                new IdentityUserRole<Guid>
                {
                    RoleId = adminRoleId,
                    UserId = adminId
                }
            );
        }
    }
}

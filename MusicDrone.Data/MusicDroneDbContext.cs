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

            var adminRoleId = Guid.NewGuid();
            var adminId = Guid.NewGuid();

            modelBuilder.Entity<ApplicationRole>().HasData(new ApplicationRole { Id = Guid.NewGuid(), Name = Constants.Roles.USERS, NormalizedName = Constants.Roles.USERS.ToUpper() });
            modelBuilder.Entity<ApplicationRole>().HasData(new ApplicationRole { Id = Guid.NewGuid(), Name = Constants.Roles.PREMIUMUSERS, NormalizedName = Constants.Roles.PREMIUMUSERS.ToUpper() });
            modelBuilder.Entity<ApplicationRole>().HasData(new ApplicationRole { Id = Guid.NewGuid(), Name = Constants.Roles.MODERATORS, NormalizedName = Constants.Roles.MODERATORS.ToUpper() });
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
                    PasswordHash = hasher.HashPassword(null, AuthorizationConstants.DEFAULT_PASSWORD)
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

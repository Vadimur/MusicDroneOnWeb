using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicDrone.Data.Models;

namespace MusicDrone.Data
{
    public class MusicDroneDbContext : IdentityDbContext<ApplicationUser>
    {
        public MusicDroneDbContext(DbContextOptions<MusicDroneDbContext> options)
            : base(options)
        {
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}

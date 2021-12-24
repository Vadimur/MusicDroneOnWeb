using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace MusicDrone.Data.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual ICollection<RoomUser> RoomsUsers { get; set; }
    }
}

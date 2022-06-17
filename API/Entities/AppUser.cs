using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace API.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Create { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public ICollection<UserLike> LikedByUsers { get; set; } // list of users who liked the currently logged in user
        public ICollection<UserLike> LikedUsers   { get; set; } // list of users the currently logged in user has liked 
        public ICollection<Message> MessagesSent { get; set; }
        public ICollection<Message> MessageRecieved { get; set; }
        public ICollection<AppUserRole> UserRoles { get; set; }

    }
}
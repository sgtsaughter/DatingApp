using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class RegisterDto
    {
        // The properties we're getting in the body of the request from the front end. 
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
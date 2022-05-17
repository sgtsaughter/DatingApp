using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.Interfaces
{
    public interface ITokenService // Every interface in C# begins with an I 
    {
        string CreateToken(AppUser user);
    }
}
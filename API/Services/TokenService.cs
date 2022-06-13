using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
  public class TokenService : ITokenService
  {
      // The same key is used to sign and verify the key.  Asymmetrical keys are when you have a public and private key because the one key doesn't need to leave the server.  
      // One used to encrypt and one used to decrypt (HTTPS, SSL).
      private readonly SymmetricSecurityKey _key; 
      public TokenService(IConfiguration config)
      {
          _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
      }
    public string CreateToken(AppUser user)
    {
      var claims = new List<Claim> 
      {
          new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()), // Name identifier.  We'll store the username claim into the JWT
          new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
      };

      var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature); 

      var tokenDescriptor = new SecurityTokenDescriptor 
        // Describe what goes into our token
        {
            Subject = new ClaimsIdentity(claims), // A list of claims from the user
            Expires = DateTime.Now.AddDays(7), // An expiration date for the token (7 days from now)
            SigningCredentials = creds // The encrypted key for the token. 
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
  }
}
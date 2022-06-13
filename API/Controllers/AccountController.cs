using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {

        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper) 
        {
            _context = context; // _context is our database. We will use await when making calls to our database.
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) 
        {
            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken"); // Returns a 400 Error response that says "Username is taken"

            var user = _mapper.Map<AppUser>(registerDto);

            using var hmac = new HMACSHA512(); // adding the using keyword will ensure the HMACSHA512 class uses the dispose method. 


            user.UserName = registerDto.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;


            _context.Users.Add(user); // Tracks user in entity framework.
            await _context.SaveChangesAsync(); // Call database and save user into user table.

            return new UserDto 
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> LoginAsync(LoginDto loginDto) 
        {
            var user = await _context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == loginDto.UserName);

            if (user == null) return Unauthorized("Invalid username"); // Returns a 401 error to the frontend

            // Calculate the computed hash of the user's password using passwordSalt 
            using var hmac = new HMACSHA512(user.PasswordSalt); // Passing in the secret key of the user's password encryption.  


            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            // If the password in computedHash is the same as the user password using the orginal salt, then the user has entered the correct password. 
            // else, they've entered the wrong password and we'll return a 401 error. 
            for (int i = 0; i < computedHash.Length; i ++) 
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password"); // Returns a 401 error to the frontend
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            }; 

        }

        private async Task<bool> UserExists(string username) 
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}
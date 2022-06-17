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

            user.UserName = registerDto.Username.ToLower();

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
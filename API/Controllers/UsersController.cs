using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    // Every method in this controller is protected by authorization. 
    [Authorize]
  public class UsersController : BaseApiController
  {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
    {
            this._userRepository = userRepository;
        }

    // always use asynchronous code when hitting the database. 
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        return Ok(await _userRepository.GetUserAsync());
    }

    // api/users/3 -- get the user with the id of 3.
    [HttpGet("{username}")]
    public async Task<ActionResult<AppUser>> GetUser(string username)
    {
            return await _userRepository.GetUserByUsernameAsync(username);
        
    }
  }
}
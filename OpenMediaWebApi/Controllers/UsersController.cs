using OpenMediaWebApi.Models;
using OpenMediaWebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace OpenMediaWebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UsersService _usersService;

        public UsersController(UsersService usersService) =>
            _usersService = usersService;

        [HttpGet]
        public async Task<List<User>> Get() =>
            await _usersService.GetAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<User>> Get(string id)
        {
            var user = await _usersService.GetAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateAsync(User user)
        {
            await _usersService.CreateAsync(user);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<ActionResult> Update(string id, User updatedUser)
        {
            var user = await _usersService.GetAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            updatedUser.Id = user.Id;

            await _usersService.UpdateAsync(id, updatedUser);
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<ActionResult> Remove(string id)
        {
            var user = await _usersService.GetAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            await _usersService.RemoveAsync(id);
            return NoContent();
        }       
    }
}

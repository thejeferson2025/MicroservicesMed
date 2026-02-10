using Microsoft.AspNetCore.Mvc;
using UserManagementService.Models;
using UserManagementService.Services;

namespace UserManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpPost]
        public async Task<ActionResult<User>> Create(User user)
        {
            var createdUser = await _service.CreateAsync(user);
            return Ok(createdUser);
        }

        // --- NUEVO ENDPOINT  ---
        [HttpPost("assign")]
        public async Task<ActionResult<User>> AssignWorkItem([FromBody] WorkItem workItem)
        {
            if (workItem == null) return BadRequest("WorkItem cannot be null");

            var assignedUser = await _service.AssignWorkItemAsync(workItem);

            if (assignedUser == null)
            {
                return StatusCode(500, "No user available to assign the task.");
            }

            return Ok(new 
            { 
                Message = $"Task assigned to {assignedUser.Name}", 
                UserLoad = assignedUser.WorkItems.Count,
                AssignedUser = assignedUser
            });
        }
    }
}
using WorkItemsService.Dtos;
using Microsoft.AspNetCore.Mvc;
using WorkItemsService.Services;

namespace WorkItemsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkItemsController : ControllerBase
    {
        private readonly IWorkItemService _service;

        public WorkItemsController(IWorkItemService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(WorkItemCreateDto dto)
        {
            try
            {
                var result = await _service.CreateAndAssignAsync(dto);
                return Ok(new { message = "Asignado exitosamente", task = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }
    }
}
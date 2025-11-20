using BackUp.Aplication.Dtos.organizacion;
using BackUp.Aplication.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;
using BackUp.Domain.Base;
using Microsoft.Extensions.Logging;

namespace BackUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrganizacionController : ControllerBase
    {
        private readonly IOrganizacionService _organizacionService;

        public OrganizacionController(IOrganizacionService organizacionService)
        {
            _organizacionService = organizacionService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ObtenerOrganizacionDTO>> ObtenerPorId(int id)
        {
            var result = await _organizacionService.ObtenerPorIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result.Message);

            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ObtenerOrganizacionDTO>>> ObtenerTodos()
        {
            var result = await _organizacionService.ObtenerTodosAsync();

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<ActionResult<int>> CrearOrganizacion(SaveOrganizacionDTO saveOrganizacion)
        {
            var result = await _organizacionService.AgregarAsync(saveOrganizacion);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return CreatedAtAction(nameof(ObtenerPorId), new { id = result.Data }, result.Data);
        }

        [HttpPut]
        public async Task<IActionResult> ActualizarOrganizacion(UpdateOrganizacionDTO updateOrganizacion)
        {
            var result = await _organizacionService.ActualizarAsync(updateOrganizacion);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarOrganizacion(int id)
        {
            var removeDto = new RemoveOrganizacionDTO { Id = id };
            var result = await _organizacionService.EliminarAsync(removeDto);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return NoContent();
        }
    }
}

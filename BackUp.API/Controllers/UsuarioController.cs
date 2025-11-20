using BackUp.Aplication.Dtos.Usuario;
using BackUp.Aplication.Interfaces.IService;
using BackUp.Domain.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<ActionResult<OperationResult>> ObtenerTodos()
        {
            var result = await _usuarioService.ObtenerTodosAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OperationResult>> ObtenerPorId(int id)
        {
            var result = await _usuarioService.ObtenerPorIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<OperationResult>> ObtenerPorEmail(string email)
        {
            var result = await _usuarioService.ObtenerPorEmailAsync(email);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<OperationResult>> Crear([FromBody] SaveUsuarioDTO saveUsuario)
        {
            if (!ModelState.IsValid)
                return BadRequest(OperationResult.Failure("Datos inválidos"));

            var result = await _usuarioService.AgregarAsync(saveUsuario);

            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(ObtenerPorId), new { id = result.Data }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OperationResult>> Actualizar(int id, [FromBody] UpdateUsuarioDTO updateUsuario)
        {
            if (id != updateUsuario.Id)
                return BadRequest(OperationResult.Failure("ID no coincide"));

            if (!ModelState.IsValid)
                return BadRequest(OperationResult.Failure("Datos inválidos"));

            var result = await _usuarioService.ActualizarAsync(updateUsuario);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<OperationResult>> Eliminar(int id)
        {
            var removeDto = new RemoveUsuarioDTO { Id = id };
            var result = await _usuarioService.EliminarAsync(removeDto);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
    }
}

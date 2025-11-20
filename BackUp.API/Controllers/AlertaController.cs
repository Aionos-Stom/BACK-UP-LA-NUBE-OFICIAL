using BackUp.Application.Dtos.Alerta;
using BackUp.Application.Interfaces.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertaController : ControllerBase
    {
        private readonly IAlertaService _alertaService;

        public AlertaController(IAlertaService alertaService)
        {
            _alertaService = alertaService;
        }

        // GET: api/alerta/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ObtenerAlertaDTO>> ObtenerPorId(int id)
        {
            var alerta = await _alertaService.ObtenerPorIdAsync(id);
            if (alerta == null)
            {
                return NotFound($"Alerta con ID {id} no encontrada");
            }
            return Ok(alerta);
        }

        // GET: api/alerta
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ObtenerAlertaDTO>>> ObtenerTodos()
        {
            var alertas = await _alertaService.ObtenerTodosAsync();
            return Ok(alertas);
        }

        // GET: api/alerta/usuario/{usuarioId}
        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<ObtenerAlertaDTO>>> ObtenerPorUsuario(int usuarioId)
        {
            var alertas = await _alertaService.ObtenerPorUsuarioAsync(usuarioId);
            return Ok(alertas);
        }

        // GET: api/alerta/no-reconocidas
        [HttpGet("no-reconocidas")]
        public async Task<ActionResult<IEnumerable<ObtenerAlertaDTO>>> ObtenerNoReconocidas()
        {
            var alertas = await _alertaService.ObtenerNoReconocidasAsync();
            return Ok(alertas);
        }

        // GET: api/alerta/job/{jobId}
        [HttpGet("job/{jobId}")]
        public async Task<ActionResult<IEnumerable<ObtenerAlertaDTO>>> ObtenerPorJob(int? jobId)
        {
            var alertas = await _alertaService.ObtenerPorJobAsync(jobId);
            return Ok(alertas);
        }

        // GET: api/alerta/severidad/{severidad}
        [HttpGet("severidad/{severidad}")]
        public async Task<ActionResult<IEnumerable<ObtenerAlertaDTO>>> ObtenerPorSeveridad(string severidad)
        {
            var alertas = await _alertaService.ObtenerPorSeveridadAsync(severidad);
            return Ok(alertas);
        }

        // POST: api/alerta
        [HttpPost]
        public async Task<ActionResult<ObtenerAlertaDTO>> Crear([FromBody] SaveAlertaDTO saveAlerta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var alertaCreada = await _alertaService.CrearAsync(saveAlerta);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = alertaCreada.Id }, alertaCreada);
        }

        // PUT: api/alerta/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ObtenerAlertaDTO>> Actualizar(int id, [FromBody] UpdateAlertaDTO updateAlerta)
        {
            if (id != updateAlerta.Id)
            {
                return BadRequest("ID de la alerta no coincide");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var alertaActualizada = await _alertaService.ActualizarAsync(updateAlerta);
            if (alertaActualizada == null)
            {
                return NotFound($"Alerta con ID {id} no encontrada");
            }

            return Ok(alertaActualizada);
        }

        // DELETE: api/alerta/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            var removeAlerta = new RemoveAlertaDTO { Id = id };
            var resultado = await _alertaService.EliminarAsync(removeAlerta);

            if (!resultado)
            {
                return NotFound($"Alerta con ID {id} no encontrada");
            }

            return NoContent();
        }

        // PATCH: api/alerta/{id}/reconocer
        [HttpPatch("{id}/reconocer")]
        public async Task<ActionResult> MarcarComoReconocida(int id)
        {
            var resultado = await _alertaService.MarcarComoReconocidaAsync(id);
            if (!resultado)
            {
                return NotFound($"Alerta con ID {id} no encontrada");
            }

            return Ok(new { message = "Alerta marcada como reconocida" });
        }

        // GET: api/alerta/contador/no-reconocidas
        [HttpGet("contador/no-reconocidas")]
        public async Task<ActionResult<int>> ContarAlertasNoReconocidas()
        {
            var contador = await _alertaService.ContarAlertasNoReconocidasAsync();
            return Ok(contador);
        }
    }
}


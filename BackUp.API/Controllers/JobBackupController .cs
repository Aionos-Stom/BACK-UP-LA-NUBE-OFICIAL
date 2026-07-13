using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackUp.Aplication.Dtos.JobBackup;
using BackUp.Aplication.Interfaces.IService;
using BackUp.Domain.Base;
using Microsoft.Extensions.Logging;

namespace BackUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JobBackupController : ControllerBase
    {
        private readonly IJobBackupService _jobBackupService;

        public JobBackupController(IJobBackupService jobBackupService)
        {
            _jobBackupService = jobBackupService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ObtenerJobBackupDTO>> ObtenerPorId(int id)
        {
            var result = await _jobBackupService.ObtenerPorIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result.Message);

            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ObtenerJobBackupDTO>>> ObtenerTodos(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
            [FromQuery] string? estado = null)
        {
            var result = await _jobBackupService.ObtenerTodosAsync();
            if (!result.IsSuccess) return BadRequest(result.Message);

            // Filtrar y paginar en memoria (el service ya carga todo)
            var jobs = (result.Data as IEnumerable<ObtenerJobBackupDTO> ?? Enumerable.Empty<ObtenerJobBackupDTO>());
            if (!string.IsNullOrEmpty(estado))
                jobs = jobs.Where(j => j.Estado == estado);

            var total = jobs.Count();
            var paginado = jobs
                .OrderByDescending(j => j.FechaProgramada)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return Ok(new { total, page, pageSize, data = paginado });
        }

        [HttpGet("programados")]
        public async Task<ActionResult<IEnumerable<ObtenerJobBackupDTO>>> ObtenerJobsProgramados()
        {
            var result = await _jobBackupService.ObtenerJobsProgramadosAsync();

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<ActionResult<int>> CrearJobBackup(SaveJobBackupDTO saveJobBackup)
        {
            var result = await _jobBackupService.AgregarAsync(saveJobBackup);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return CreatedAtAction(nameof(ObtenerPorId), new { id = result.Data }, result.Data);
        }

        [HttpPut]
        public async Task<IActionResult> ActualizarJobBackup(UpdateJobBackupDTO updateJobBackup)
        {
            var result = await _jobBackupService.ActualizarAsync(updateJobBackup);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarJobBackup(int id)
        {
            var removeDto = new RemoveJobBackupDTO { Id = id };
            var result = await _jobBackupService.EliminarAsync(removeDto);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return NoContent();
        }

        [HttpPost("{id}/ejecutar")]
        public async Task<IActionResult> EjecutarJob(int id)
        {
            var result = await _jobBackupService.EjecutarJobAsync(id);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok("Job ejecutado correctamente");
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using BackUp.Aplication.Dtos.JobBackup;
using BackUp.Aplication.Interfaces.IService;
using BackUp.Domain.Base;
using Microsoft.Extensions.Logging;

namespace BackUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<ActionResult<IEnumerable<ObtenerJobBackupDTO>>> ObtenerTodos()
        {
            var result = await _jobBackupService.ObtenerTodosAsync();

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Data);
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
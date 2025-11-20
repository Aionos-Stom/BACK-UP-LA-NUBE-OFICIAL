using BackUp.Aplication.Dtos.CloudStorage;
using BackUp.Aplication.Interfaces.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CloudStorageController : ControllerBase
    {
        private readonly ICloudStorageService _cloudStorageService;

        public CloudStorageController(ICloudStorageService cloudStorageService)
        {
            _cloudStorageService = cloudStorageService;
        }

        // GET: api/cloudstorage/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ObtenerCloudStorageDTO>> ObtenerPorId(int id)
        {
            var result = await _cloudStorageService.ObtenerPorIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result.Message);

            return Ok(result.Data);
        }

        // GET: api/cloudstorage
        [HttpGet]
        public async Task<ActionResult> ObtenerTodos()
        {
            var result = await _cloudStorageService.ObtenerTodosAsync();

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        // GET: api/cloudstorage/organizacion/{organizacionId}
        [HttpGet("organizacion/{organizacionId}")]
        public async Task<ActionResult> ObtenerPorOrganizacion(int organizacionId)
        {
            var result = await _cloudStorageService.ObtenerPorOrganizacionAsync(organizacionId);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        // POST: api/cloudstorage
        [HttpPost]
        public async Task<ActionResult> Crear([FromBody] SaveCloudStorageDTO saveCloudStorage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _cloudStorageService.AgregarAsync(saveCloudStorage);

            if (!result.IsSuccess)
                return BadRequest(result.Message);
// Validar que result.Data no sea null antes de castear
    if (result.Data == null)
    {
        return BadRequest("No se pudo crear el cloud storage");
    }

    var createdDto = result.Data as ObtenerCloudStorageDTO;
    if (createdDto == null)
    {
        return BadRequest("Tipo de dato inválido al crear cloud storage");
    }

    return CreatedAtAction(nameof(ObtenerPorId), new { id = createdDto.Id }, result.Data);
        }

        // PUT: api/cloudstorage/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Actualizar(int id, [FromBody] UpdateCloudStorageDTO updateCloudStorage)
        {
            if (id != updateCloudStorage.Id)
            {
                return BadRequest("ID del cloud storage no coincide");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _cloudStorageService.ActualizarAsync(updateCloudStorage);

            if (!result.IsSuccess)
                return NotFound(result.Message);

            return Ok(result.Data);
        }

        // DELETE: api/cloudstorage/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            var removeCloudStorage = new RemoveCloudStorageDTO { Id = id };
            var result = await _cloudStorageService.EliminarAsync(removeCloudStorage);

            if (!result.IsSuccess)
                return NotFound(result.Message);

            return NoContent();
        }
    }
}

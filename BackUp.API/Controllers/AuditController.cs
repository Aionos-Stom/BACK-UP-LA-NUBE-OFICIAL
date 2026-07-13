using BackUp.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackUp.API.Controllers
{
    [ApiController]
    [Route("api/audit")]
    [Authorize(Roles = "admin,superadmin")]
    public class AuditController : ControllerBase
    {
        private readonly BackUpDbContext _context;
        public AuditController(BackUpDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetLogs(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25,
            [FromQuery] string? accion = null,
            [FromQuery] string? email = null,
            [FromQuery] string? ip = null,
            [FromQuery] string? navegador = null,
            [FromQuery] string? dispositivo = null,
            [FromQuery] DateTime? desde = null,
            [FromQuery] DateTime? hasta = null)
        {
            var query = _context.AuditLog.AsQueryable();

            if (!string.IsNullOrEmpty(accion))
                query = query.Where(l => l.Accion.Contains(accion));
            if (!string.IsNullOrEmpty(email))
                query = query.Where(l => l.Email != null && l.Email.Contains(email));
            if (!string.IsNullOrEmpty(ip))
                query = query.Where(l => l.IpAddress != null && l.IpAddress.Contains(ip));
            if (!string.IsNullOrEmpty(navegador))
                query = query.Where(l => l.Navegador != null && l.Navegador.Contains(navegador));
            if (!string.IsNullOrEmpty(dispositivo))
                query = query.Where(l => l.Dispositivo != null && l.Dispositivo == dispositivo);
            if (desde.HasValue)
                query = query.Where(l => l.CreadoEn >= desde.Value);
            if (hasta.HasValue)
                query = query.Where(l => l.CreadoEn <= hasta.Value.AddDays(1));

            var total = await query.CountAsync();
            var logs = await query
                .OrderByDescending(l => l.CreadoEn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new
                {
                    l.Id,
                    l.Accion,
                    l.DescripcionLegible,
                    l.Entidad,
                    l.EntidadId,
                    l.Email,
                    l.UsuarioId,
                    l.IpAddress,
                    l.Navegador,
                    l.SistemaOperativo,
                    l.Dispositivo,
                    l.UserAgent,
                    l.DatosAnteriores,
                    l.DatosNuevos,
                    l.CreadoEn
                })
                .ToListAsync();

            return Ok(new { total, page, pageSize, logs });
        }

        [HttpGet("acciones")]
        public async Task<IActionResult> GetAcciones()
        {
            var acciones = await _context.AuditLog
                .Select(l => l.Accion)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();
            return Ok(acciones);
        }
    }
}

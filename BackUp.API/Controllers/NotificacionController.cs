using BackUp.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackUp.API.Controllers
{
    [ApiController]
    [Route("api/notificaciones")]
    [Authorize]
    public class NotificacionController : ControllerBase
    {
        private readonly BackUpDbContext _context;
        public NotificacionController(BackUpDbContext context) => _context = context;

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? "0");
        private bool IsAdmin => User.IsInRole("admin") || User.IsInRole("superadmin");

        [HttpGet]
        public async Task<IActionResult> GetNotificaciones()
        {
            var userId = GetUserId();
            var alertas = IsAdmin
                ? await _context.Alerta
                    .Where(a => !a.IsAcknowledged)
                    .OrderByDescending(a => a.Id)
                    .Take(20)
                    .Select(a => new { a.Id, tipoAlerta = a.Tipo, a.Severidad, a.Mensaje, categoria = "alerta" })
                    .ToListAsync()
                : await _context.Alerta
                    .Where(a => a.UsuarioId == userId && !a.IsAcknowledged)
                    .OrderByDescending(a => a.Id)
                    .Take(10)
                    .Select(a => new { a.Id, tipoAlerta = a.Tipo, a.Severidad, a.Mensaje, categoria = "alerta" })
                    .ToListAsync();

            var contador = alertas.Count;
            return Ok(new { contador, notificaciones = alertas });
        }

        [HttpPost("{id}/leer")]
        public async Task<IActionResult> MarcarLeida(int id)
        {
            var alerta = await _context.Alerta.FindAsync(id);
            if (alerta == null) return NotFound();
            alerta.IsAcknowledged = true;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("leer-todas")]
        public async Task<IActionResult> MarcarTodasLeidas()
        {
            var userId = GetUserId();
            var alertas = IsAdmin
                ? await _context.Alerta.Where(a => !a.IsAcknowledged).ToListAsync()
                : await _context.Alerta.Where(a => a.UsuarioId == userId && !a.IsAcknowledged).ToListAsync();
            alertas.ForEach(a => a.IsAcknowledged = true);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"{alertas.Count} notificaciones marcadas como leídas." });
        }
    }
}

using BackUp.Domain.Entities.Bac;
using BackUp.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackUp.API.Controllers
{
    [ApiController]
    [Route("api/planes")]
    public class PlanController : ControllerBase
    {
        private readonly BackUpDbContext _context;
        public PlanController(BackUpDbContext context) => _context = context;

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPlanes()
        {
            var planes = await _context.Plan
                .Where(p => p.IsActive)
                .OrderBy(p => p.PrecioMensual)
                .ToListAsync();
            return Ok(planes);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPlan(int id)
        {
            var plan = await _context.Plan.FindAsync(id);
            if (plan == null) return NotFound();
            return Ok(plan);
        }

        [Authorize(Roles = "admin,superadmin")]
        [HttpPost]
        public async Task<IActionResult> CrearPlan([FromBody] Plan plan)
        {
            _context.Plan.Add(plan);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPlan), new { id = plan.Id }, plan);
        }

        [Authorize(Roles = "admin,superadmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarPlan(int id, [FromBody] Plan planActualizado)
        {
            var plan = await _context.Plan.FindAsync(id);
            if (plan == null) return NotFound();
            plan.Nombre = planActualizado.Nombre;
            plan.Descripcion = planActualizado.Descripcion;
            plan.PrecioMensual = planActualizado.PrecioMensual;
            plan.LimiteAlmacenamientoBytes = planActualizado.LimiteAlmacenamientoBytes;
            plan.MaxJobsConcurrentes = planActualizado.MaxJobsConcurrentes;
            plan.MaxPoliticas = planActualizado.MaxPoliticas;
            plan.BackupAutomatico = planActualizado.BackupAutomatico;
            plan.SoportePrioritario = planActualizado.SoportePrioritario;
            await _context.SaveChangesAsync();
            return Ok(plan);
        }

        [Authorize]
        [HttpPost("{planId}/suscribir")]
        public async Task<IActionResult> Suscribir(int planId, [FromBody] SuscribirRequest req)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? "0");
            var plan = await _context.Plan.FindAsync(planId);
            if (plan == null) return NotFound(new { message = "Plan no encontrado." });
            if (plan.EsGratuito && plan.PrecioMensual == 0)
                return BadRequest(new { message = "El plan gratuito se asigna automáticamente al registrarse." });

            var suscActual = await _context.Suscripcion
                .FirstOrDefaultAsync(s => s.UsuarioId == userId && (s.Estado == "activa" || s.Estado == "gratis_admin"));
            if (suscActual != null && suscActual.EsGratisAdminGranted)
                return BadRequest(new { message = "Tienes un plan gratuito otorgado por un administrador. Contacta soporte para cambiarlo." });
            if (suscActual != null && !suscActual.EsGratisAdminGranted)
            {
                suscActual.Estado = "cancelada";
                suscActual.FechaFin = DateTime.UtcNow;
            }

            var nueva = new Suscripcion
            {
                UsuarioId = userId,
                PlanId = planId,
                Estado = "activa",
                FechaInicio = DateTime.UtcNow,
                FechaFin = DateTime.UtcNow.AddMonths(1)
            };
            _context.Suscripcion.Add(nueva);
            await _context.SaveChangesAsync();

            var pago = new Pago
            {
                SuscripcionId = nueva.Id,
                Monto = plan.PrecioMensual,
                Estado = "completado",
                MetodoPago = req.MetodoPago ?? "tarjeta",
                Descripcion = $"Suscripción plan {plan.Nombre}"
            };
            _context.Pago.Add(pago);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Suscrito exitosamente al plan {plan.Nombre}.", suscripcionId = nueva.Id });
        }

        [Authorize]
        [HttpGet("mi-suscripcion")]
        public async Task<IActionResult> MiSuscripcion()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? "0");
            var susc = await _context.Suscripcion
                .Include(s => s.Plan)
                .Where(s => s.UsuarioId == userId && (s.Estado == "activa" || s.Estado == "gratis_admin"))
                .OrderByDescending(s => s.FechaInicio)
                .FirstOrDefaultAsync();
            if (susc == null) return Ok(new { mensaje = "Sin suscripción activa." });
            return Ok(new
            {
                plan = susc.Plan?.Nombre,
                estado = susc.Estado,
                fechaFin = susc.FechaFin,
                esGratisAdmin = susc.EsGratisAdminGranted,
                almacenamientoLimiteGB = susc.Plan != null ? Math.Round(susc.Plan.LimiteAlmacenamientoBytes / 1073741824.0, 2) : 0,
                almacenamientoUsadoGB = Math.Round(susc.AlmacenamientoUsadoBytes / 1073741824.0, 2)
            });
        }
    }

    public record SuscribirRequest(string? MetodoPago);
}

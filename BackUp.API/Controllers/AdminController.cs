using BackUp.API.Services;
using BackUp.Domain.Entities.Bac;
using BackUp.Domainn.Entities.Users;
using BackUp.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackUp.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "admin,superadmin")]
    public class AdminController : ControllerBase
    {
        private readonly BackUpDbContext _context;
        private readonly IAuditService _audit;

        public AdminController(BackUpDbContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        // --- USUARIOS ---
        [HttpGet("usuarios")]
        public async Task<IActionResult> GetUsuarios()
        {
            var usuarios = await _context.Usuario
                .Include(u => u.Organizacion)
                .ToListAsync();

            var userIds = usuarios.Select(u => u.Id).ToList();
            var suscripciones = await _context.Suscripcion
                .Include(s => s.Plan)
                .Where(s => userIds.Contains(s.UsuarioId) && (s.Estado == "activa" || s.Estado == "gratis_admin"))
                .OrderByDescending(s => s.FechaInicio)
                .ToListAsync();

            // Tomar la suscripción más reciente por usuario
            var suscripcionPorUsuario = suscripciones
                .GroupBy(s => s.UsuarioId)
                .ToDictionary(g => g.Key, g => g.First());

            var result = usuarios.Select(u =>
            {
                suscripcionPorUsuario.TryGetValue(u.Id, out var susc);
                return new
                {
                    id = u.Id,
                    nombre = u.Nombre,
                    email = u.Email,
                    rol = u.Rol,
                    isActive = u.IsActive,
                    lastLogin = u.LastLogin,
                    organizacion = u.Organizacion?.Nombre ?? "N/A",
                    plan = susc?.Plan?.Nombre ?? "Sin plan",
                    suscripcionEstado = susc?.Estado ?? "N/A",
                    esGratisAdmin = susc?.EsGratisAdminGranted ?? false
                };
            }).ToList();

            return Ok(result);
        }

        [HttpPut("usuarios/{id}/rol")]
        public async Task<IActionResult> CambiarRol(int id, [FromBody] CambiarRolRequest req)
        {
            var rolesValidos = new[] { "usuario", "auditor", "admin" };
            if (!rolesValidos.Contains(req.NuevoRol))
                return BadRequest(new { message = "Rol inválido." });

            var miRol = User.FindFirstValue(ClaimTypes.Role);
            if (req.NuevoRol == "admin" && miRol != "superadmin")
                return Forbid();

            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null) return NotFound();
            var rolAnterior = usuario.Rol;
            usuario.Rol = req.NuevoRol;
            await _context.SaveChangesAsync();
            await _audit.LogAsync("CAMBIO_ROL", "Usuario", id.ToString(), new { rolAnterior }, new { nuevoRol = req.NuevoRol });
            return Ok(new { message = $"Rol actualizado a {req.NuevoRol}." });
        }

        [HttpPut("usuarios/{id}/activar")]
        public async Task<IActionResult> ActivarUsuario(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null) return NotFound();
            usuario.IsActive = true;
            await _context.SaveChangesAsync();
            await _audit.LogAsync("ACTIVAR_USUARIO", "Usuario", id.ToString());
            return Ok(new { message = "Usuario activado." });
        }

        [HttpPut("usuarios/{id}/desactivar")]
        public async Task<IActionResult> DesactivarUsuario(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null) return NotFound();
            usuario.IsActive = false;
            await _context.SaveChangesAsync();
            await _audit.LogAsync("DESACTIVAR_USUARIO", "Usuario", id.ToString());
            return Ok(new { message = "Usuario desactivado." });
        }

        // --- PLANES / SUSCRIPCIONES ---
        [HttpPost("usuarios/{id}/otorgar-plan-gratis")]
        public async Task<IActionResult> OtorgarPlanGratis(int id, [FromBody] OtorgarPlanRequest req)
        {
            var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? "0");
            var usuario = await _context.Usuario.FindAsync(id);
            var plan = await _context.Plan.FindAsync(req.PlanId);
            if (usuario == null || plan == null) return NotFound();

            // Bloquear duplicado: ya tiene este plan activo asignado por admin
            var yaAsignado = await _context.Suscripcion
                .AnyAsync(s => s.UsuarioId == id && s.PlanId == req.PlanId &&
                               (s.Estado == "gratis_admin" || s.Estado == "activa") &&
                               s.EsGratisAdminGranted);
            if (yaAsignado)
                return BadRequest(new { message = $"El usuario '{usuario.Nombre}' ya tiene el plan '{plan.Nombre}' asignado gratuitamente." });

            // Cancelar suscripción activa anterior
            var suscActual = await _context.Suscripcion
                .FirstOrDefaultAsync(s => s.UsuarioId == id && (s.Estado == "activa" || s.Estado == "gratis_admin"));
            if (suscActual != null) { suscActual.Estado = "cancelada"; suscActual.FechaFin = DateTime.UtcNow; }

            var nueva = new Suscripcion
            {
                UsuarioId = id,
                PlanId = req.PlanId,
                Estado = "gratis_admin",
                FechaInicio = DateTime.UtcNow,
                FechaFin = req.FechaFin,
                EsGratisAdminGranted = true,
                OtorgadaPorAdminId = adminId
            };
            _context.Suscripcion.Add(nueva);
            await _context.SaveChangesAsync();
            await _audit.LogAsync("OTORGAR_PLAN_GRATIS", "Suscripcion", id.ToString(), null, new { planId = req.PlanId });
            return Ok(new { message = $"Plan '{plan.Nombre}' otorgado gratis al usuario {usuario.Nombre}." });
        }

        [HttpPost("usuarios/{id}/revocar-plan-gratis")]
        public async Task<IActionResult> RevocarPlanGratis(int id)
        {
            var susc = await _context.Suscripcion
                .FirstOrDefaultAsync(s => s.UsuarioId == id && s.EsGratisAdminGranted && s.Estado == "gratis_admin");
            if (susc == null)
                return NotFound(new { message = "No tiene un plan gratuito admin activo." });

            susc.Estado = "cancelada";
            susc.FechaFin = DateTime.UtcNow;

            var planGratuito = await _context.Plan.FirstOrDefaultAsync(p => p.EsGratuito && p.IsActive);
            if (planGratuito != null)
            {
                _context.Suscripcion.Add(new Suscripcion
                {
                    UsuarioId = id,
                    PlanId = planGratuito.Id,
                    Estado = "activa",
                    FechaInicio = DateTime.UtcNow
                });
            }
            await _context.SaveChangesAsync();
            return Ok(new { message = "Plan gratuito admin revocado." });
        }

        // --- ESTADÍSTICAS ADMIN ---
        [HttpGet("estadisticas")]
        public async Task<IActionResult> GetEstadisticas()
        {
            var totalUsuarios = await _context.Usuario.CountAsync();
            var usuariosActivos = await _context.Usuario.CountAsync(u => u.IsActive);
            var totalJobs = await _context.JobBackup.CountAsync();
            var jobsCompletados = await _context.JobBackup.CountAsync(j => j.Estado == "completado");
            var ingresosMes = await _context.Pago
                .Where(p => p.Estado == "completado" && p.FechaPago >= DateTime.UtcNow.AddMonths(-1))
                .SumAsync(p => (decimal?)p.Monto) ?? 0;
            var suscripcionesPorPlan = await _context.Suscripcion
                .Where(s => s.Estado == "activa" || s.Estado == "gratis_admin")
                .Include(s => s.Plan)
                .GroupBy(s => s.Plan!.Nombre)
                .Select(g => new { plan = g.Key, cantidad = g.Count() })
                .ToListAsync();

            return Ok(new
            {
                totalUsuarios,
                usuariosActivos,
                totalJobs,
                jobsCompletados,
                ingresosMes,
                suscripcionesPorPlan
            });
        }

        [HttpGet("pagos")]
        public async Task<IActionResult> GetPagos([FromQuery] string? filtro = null)
        {
            var query = _context.Pago
                .Include(p => p.Suscripcion).ThenInclude(s => s!.Usuario)
                .Include(p => p.Suscripcion).ThenInclude(s => s!.Plan)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filtro))
                query = query.Where(p => p.Suscripcion!.Usuario!.Nombre.Contains(filtro) ||
                                         p.Suscripcion.Usuario.Email.Contains(filtro));

            var pagos = await query
                .OrderByDescending(p => p.FechaPago)
                .Take(200)
                .Select(p => new
                {
                    id = p.Id,
                    monto = p.Monto,
                    moneda = p.Moneda,
                    estado = p.Estado,
                    metodoPago = p.MetodoPago,
                    fechaPago = p.FechaPago,
                    usuario = p.Suscripcion!.Usuario!.Nombre,
                    emailUsuario = p.Suscripcion.Usuario.Email,
                    plan = p.Suscripcion.Plan!.Nombre
                })
                .ToListAsync();
            return Ok(pagos);
        }

        // --- DETALLE USUARIO ---
        [HttpGet("usuarios/{id}/detalle")]
        public async Task<IActionResult> GetDetalleUsuario(int id)
        {
            var usuario = await _context.Usuario
                .Include(u => u.Organizacion)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null) return NotFound();

            var suscripciones = await _context.Suscripcion
                .Include(s => s.Plan)
                .Where(s => s.UsuarioId == id)
                .OrderByDescending(s => s.FechaInicio)
                .Select(s => new { s.Id, plan = s.Plan!.Nombre, s.Estado, s.FechaInicio, s.FechaFin, s.EsGratisAdminGranted })
                .ToListAsync();

            var sesionesActivas = await _context.RefreshToken
                .Where(rt => rt.UsuarioId == id && !rt.Revocado && rt.Expiracion > DateTime.UtcNow)
                .CountAsync();

            var totalPagos = await _context.Pago
                .Include(p => p.Suscripcion)
                .Where(p => p.Suscripcion!.UsuarioId == id && p.Estado == "completado")
                .SumAsync(p => (decimal?)p.Monto) ?? 0;

            var totalJobs = await _context.JobBackup.CountAsync();

            return Ok(new
            {
                id = usuario.Id,
                nombre = usuario.Nombre,
                email = usuario.Email,
                rol = usuario.Rol,
                isActive = usuario.IsActive,
                lastLogin = usuario.LastLogin,
                telefono = usuario.PhoneNumber,
                organizacion = usuario.Organizacion?.Nombre,
                fotoPerfil = usuario.FotoPerfil,
                bio = usuario.Bio,
                ciudad = usuario.Ciudad,
                pais = usuario.Pais,
                cargo = usuario.Cargo,
                empresa = usuario.Empresa,
                sesionesActivas,
                totalPagos,
                totalJobs,
                suscripciones
            });
        }

        // --- FORZAR LOGOUT ---
        [HttpDelete("usuarios/{id}/sesiones")]
        public async Task<IActionResult> ForzarLogout(int id)
        {
            var tokens = await _context.RefreshToken
                .Where(rt => rt.UsuarioId == id && !rt.Revocado)
                .ToListAsync();
            tokens.ForEach(t => t.Revocado = true);
            await _context.SaveChangesAsync();
            await _audit.LogAsync("FORZAR_LOGOUT", "RefreshToken", id.ToString());
            return Ok(new { message = $"{tokens.Count} sesión(es) cerradas forzosamente." });
        }

        // --- CREAR USUARIO DESDE ADMIN ---
        [HttpPost("usuarios")]
        public async Task<IActionResult> CrearUsuario([FromBody] CrearUsuarioRequest req)
        {
            if (await _context.Usuario.AnyAsync(u => u.Email == req.Email))
                return BadRequest(new { message = "El email ya está en uso." });

            var planGratuito = await _context.Plan.FirstOrDefaultAsync(p => p.EsGratuito && p.IsActive);
            var usuario = new Usuario
            {
                Email = req.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password, workFactor: 12),
                Nombre = req.Nombre,
                Rol = req.Rol ?? "usuario",
                OrganizacionId = 1,
                IsActive = true
            };
            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();

            if (planGratuito != null)
            {
                _context.Suscripcion.Add(new Suscripcion
                {
                    UsuarioId = usuario.Id,
                    PlanId = planGratuito.Id,
                    Estado = "activa",
                    FechaInicio = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
            }

            await _audit.LogAsync("CREAR_USUARIO_ADMIN", "Usuario", usuario.Id.ToString(), null, new { req.Email, req.Rol });
            return Ok(new { message = $"Usuario '{req.Nombre}' creado.", id = usuario.Id });
        }
    }

    public record CambiarRolRequest(string NuevoRol);
    public record OtorgarPlanRequest(int PlanId, DateTime? FechaFin);
    public record CrearUsuarioRequest(string Email, string Password, string Nombre, string? Rol);
}

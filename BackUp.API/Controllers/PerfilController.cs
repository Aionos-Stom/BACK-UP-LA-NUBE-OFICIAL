using BackUp.API.Services;
using BackUp.Domain.Entities.Bac;
using BackUp.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackUp.API.Controllers
{
    [ApiController]
    [Route("api/perfil")]
    [Authorize]
    public class PerfilController : ControllerBase
    {
        private readonly BackUpDbContext _context;
        private readonly IAuditService _audit;

        public PerfilController(BackUpDbContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? "0");

        [HttpGet]
        public async Task<IActionResult> GetPerfil()
        {
            var id = GetUserId();
            var usuario = await _context.Usuario
                .Include(u => u.Organizacion)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null) return NotFound();

            var suscripcion = await _context.Suscripcion
                .Include(s => s.Plan)
                .Where(s => s.UsuarioId == id && (s.Estado == "activa" || s.Estado == "gratis_admin"))
                .OrderByDescending(s => s.FechaInicio)
                .FirstOrDefaultAsync();

            var sesionesActivas = await _context.RefreshToken
                .Where(rt => rt.UsuarioId == id && !rt.Revocado && rt.Expiracion > DateTime.UtcNow)
                .CountAsync();

            return Ok(new
            {
                id = usuario.Id,
                nombre = usuario.Nombre,
                email = usuario.Email,
                rol = usuario.Rol,
                telefono = usuario.PhoneNumber,
                lastLogin = usuario.LastLogin,
                organizacion = usuario.Organizacion?.Nombre,
                mfaHabilitado = usuario.MfaHabilitado,
                sesionesActivas,
                fotoPerfil = usuario.FotoPerfil,
                bio = usuario.Bio,
                ciudad = usuario.Ciudad,
                pais = usuario.Pais,
                cargo = usuario.Cargo,
                empresa = usuario.Empresa,
                fechaNacimiento = usuario.FechaNacimiento,
                linkedin = usuario.LinkedIn,
                plan = suscripcion?.Plan == null ? null : new
                {
                    nombre = suscripcion.Plan.Nombre,
                    estado = suscripcion.Estado,
                    fechaFin = suscripcion.FechaFin,
                    esGratisAdmin = suscripcion.EsGratisAdminGranted,
                    limiteGB = Math.Round(suscripcion.Plan.LimiteAlmacenamientoBytes / 1073741824.0, 2),
                    usadoGB = Math.Round(suscripcion.AlmacenamientoUsadoBytes / 1073741824.0, 4),
                    porcentajeUso = suscripcion.Plan.LimiteAlmacenamientoBytes > 0
                        ? Math.Round((double)suscripcion.AlmacenamientoUsadoBytes / suscripcion.Plan.LimiteAlmacenamientoBytes * 100, 2)
                        : 0
                }
            });
        }

        [HttpPut]
        public async Task<IActionResult> ActualizarPerfil([FromBody] ActualizarPerfilRequest req)
        {
            var id = GetUserId();
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null) return NotFound();

            var anterior = new { usuario.Nombre, usuario.PhoneNumber };
            usuario.Nombre = req.Nombre ?? usuario.Nombre;
            usuario.PhoneNumber = req.Telefono ?? usuario.PhoneNumber;
            usuario.Bio = req.Bio ?? usuario.Bio;
            usuario.Ciudad = req.Ciudad ?? usuario.Ciudad;
            usuario.Pais = req.Pais ?? usuario.Pais;
            usuario.Cargo = req.Cargo ?? usuario.Cargo;
            usuario.Empresa = req.Empresa ?? usuario.Empresa;
            usuario.LinkedIn = req.LinkedIn ?? usuario.LinkedIn;
            if (req.FechaNacimiento.HasValue) usuario.FechaNacimiento = req.FechaNacimiento;

            await _context.SaveChangesAsync();
            await _audit.LogAsync("ACTUALIZAR_PERFIL", "Usuario", id.ToString(), anterior, new { usuario.Nombre });
            return Ok(new { message = "Perfil actualizado correctamente." });
        }

        [HttpPost("foto")]
        public async Task<IActionResult> SubirFoto([FromBody] FotoRequest req)
        {
            if (string.IsNullOrEmpty(req.FotoBase64))
                return BadRequest(new { message = "La imagen no puede estar vacía." });

            // Validar que sea base64 de imagen y límite ~400KB
            if (!req.FotoBase64.StartsWith("data:image/"))
                return BadRequest(new { message = "Formato de imagen inválido. Debe ser base64 con prefijo data:image/." });

            if (req.FotoBase64.Length > 600_000)
                return BadRequest(new { message = "La imagen no puede superar 450KB. Reduce el tamaño antes de subir." });

            var id = GetUserId();
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null) return NotFound();

            usuario.FotoPerfil = req.FotoBase64;
            await _context.SaveChangesAsync();
            await _audit.LogAsync("SUBIR_FOTO", "Usuario", id.ToString());
            return Ok(new { message = "Foto de perfil actualizada." });
        }

        [HttpDelete("foto")]
        public async Task<IActionResult> EliminarFoto()
        {
            var id = GetUserId();
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null) return NotFound();
            usuario.FotoPerfil = null;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Foto eliminada." });
        }

        [HttpGet("sesiones")]
        public async Task<IActionResult> GetSesiones()
        {
            var id = GetUserId();
            var tokens = await _context.RefreshToken
                .Where(rt => rt.UsuarioId == id && !rt.Revocado && rt.Expiracion > DateTime.UtcNow)
                .OrderByDescending(rt => rt.CreadoEn)
                .Select(rt => new { rt.Id, rt.CreadoEn, rt.Expiracion })
                .ToListAsync();
            return Ok(tokens);
        }

        [HttpDelete("sesiones/{tokenId}")]
        public async Task<IActionResult> RevocarSesion(int tokenId)
        {
            var id = GetUserId();
            var token = await _context.RefreshToken.FirstOrDefaultAsync(rt => rt.Id == tokenId && rt.UsuarioId == id);
            if (token == null) return NotFound();
            token.Revocado = true;
            await _context.SaveChangesAsync();
            await _audit.LogAsync("REVOCAR_SESION", "RefreshToken", tokenId.ToString());
            return Ok(new { message = "Sesión revocada." });
        }

        [HttpDelete("sesiones")]
        public async Task<IActionResult> RevocarTodasSesiones()
        {
            var id = GetUserId();
            var tokens = await _context.RefreshToken
                .Where(rt => rt.UsuarioId == id && !rt.Revocado)
                .ToListAsync();
            tokens.ForEach(t => t.Revocado = true);
            await _context.SaveChangesAsync();
            await _audit.LogAsync("REVOCAR_TODAS_SESIONES", "RefreshToken", id.ToString());
            return Ok(new { message = $"{tokens.Count} sesiones revocadas." });
        }

        [HttpGet("historial-pagos")]
        public async Task<IActionResult> GetHistorialPagos()
        {
            var id = GetUserId();
            var pagos = await _context.Pago
                .Include(p => p.Suscripcion).ThenInclude(s => s!.Plan)
                .Where(p => p.Suscripcion!.UsuarioId == id)
                .OrderByDescending(p => p.FechaPago)
                .Select(p => new
                {
                    p.Id, p.Monto, p.Moneda, p.Estado,
                    p.MetodoPago, p.FechaPago, p.Descripcion,
                    plan = p.Suscripcion!.Plan!.Nombre
                })
                .ToListAsync();
            return Ok(pagos);
        }
    }

    public record ActualizarPerfilRequest(
        string? Nombre, string? Telefono, string? Bio,
        string? Ciudad, string? Pais, string? Cargo,
        string? Empresa, string? LinkedIn, DateTime? FechaNacimiento);

    public record FotoRequest(string FotoBase64);
}

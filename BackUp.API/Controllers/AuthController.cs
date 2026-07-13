using BackUp.API.Services;
using BackUp.Domain.Entities.Bac;
using BackUp.Domainn.Entities.Users;
using BackUp.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackUp.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly BackUpDbContext _context;
        private readonly IJwtService _jwt;
        private readonly IAuditService _audit;

        public AuthController(BackUpDbContext context, IJwtService jwt, IAuditService audit)
        {
            _context = context;
            _jwt = jwt;
            _audit = audit;
        }

        private void SetAuthCookies(string accessToken, string refreshToken)
        {
            Response.Cookies.Append("access_token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // cambiar a true en producción con HTTPS
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddMinutes(60)
            });
            Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                Path = "/api/auth"
            });
        }

        private void ClearAuthCookies()
        {
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token", new CookieOptions { Path = "/api/auth" });
        }

        [HttpPost("login")]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.Email == req.Email && u.IsActive);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(req.Password, usuario.PasswordHash))
                return Unauthorized(new { message = "Credenciales inválidas." });

            usuario.LastLogin = DateTime.UtcNow;

            var suscripcionActiva = await _context.Suscripcion
                .Include(s => s.Plan)
                .Where(s => s.UsuarioId == usuario.Id && (s.Estado == "activa" || s.Estado == "gratis_admin"))
                .OrderByDescending(s => s.FechaInicio)
                .FirstOrDefaultAsync();

            var refreshTokenEntity = new RefreshToken
            {
                UsuarioId = usuario.Id,
                Token = _jwt.GenerarRefreshToken(),
                Expiracion = DateTime.UtcNow.AddDays(7)
            };
            _context.RefreshToken.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            await _audit.LogAsync("LOGIN", "Usuario", usuario.Id.ToString(), null, new { usuario.Email, usuario.Rol });

            var accessToken = _jwt.GenerarAccessToken(usuario.Id, usuario.Email, usuario.Nombre, usuario.Rol);
            SetAuthCookies(accessToken, refreshTokenEntity.Token);

            return Ok(new
            {
                usuario = new
                {
                    id = usuario.Id,
                    nombre = usuario.Nombre,
                    email = usuario.Email,
                    rol = usuario.Rol,
                    plan = suscripcionActiva?.Plan?.Nombre ?? "Sin plan"
                }
            });
        }

        [HttpPost("register")]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            if (await _context.Usuario.AnyAsync(u => u.Email == req.Email))
                return BadRequest(new { message = "El email ya está registrado." });

            var planGratuito = await _context.Plan.FirstOrDefaultAsync(p => p.EsGratuito && p.IsActive);

            var usuario = new Usuario
            {
                Email = req.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password, workFactor: 12),
                Nombre = req.Nombre,
                Rol = "usuario",
                OrganizacionId = req.OrganizacionId > 0 ? req.OrganizacionId : 1,
                IsActive = true,
                Ciudad = req.Ciudad,
                Pais = req.Pais,
                Cargo = req.Cargo,
                Empresa = req.Empresa,
            };
            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();

            if (planGratuito != null)
            {
                var suscripcion = new Suscripcion
                {
                    UsuarioId = usuario.Id,
                    PlanId = planGratuito.Id,
                    Estado = "activa",
                    FechaInicio = DateTime.UtcNow
                };
                _context.Suscripcion.Add(suscripcion);
                await _context.SaveChangesAsync();
            }

            var refreshTokenEntity = new RefreshToken
            {
                UsuarioId = usuario.Id,
                Token = _jwt.GenerarRefreshToken(),
                Expiracion = DateTime.UtcNow.AddDays(7)
            };
            _context.RefreshToken.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            await _audit.LogAsync("REGISTER", "Usuario", usuario.Id.ToString(), null, new { usuario.Email });

            var accessToken = _jwt.GenerarAccessToken(usuario.Id, usuario.Email, usuario.Nombre, usuario.Rol);
            SetAuthCookies(accessToken, refreshTokenEntity.Token);

            return Ok(new { usuarioId = usuario.Id });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshTokenCookie = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshTokenCookie))
                return Unauthorized(new { message = "No refresh token." });

            var rt = await _context.RefreshToken
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(r => r.Token == refreshTokenCookie && !r.Revocado);

            if (rt == null || !rt.EstaActivo)
            {
                ClearAuthCookies();
                return Unauthorized(new { message = "Refresh token inválido o expirado." });
            }

            rt.Revocado = true;
            var nuevoRt = new RefreshToken
            {
                UsuarioId = rt.UsuarioId,
                Token = _jwt.GenerarRefreshToken(),
                Expiracion = DateTime.UtcNow.AddDays(7)
            };
            rt.ReemplazadoPor = nuevoRt.Token;
            _context.RefreshToken.Add(nuevoRt);
            await _context.SaveChangesAsync();

            var accessToken = _jwt.GenerarAccessToken(rt.Usuario!.Id, rt.Usuario.Email, rt.Usuario.Nombre, rt.Usuario.Rol);
            SetAuthCookies(accessToken, nuevoRt.Token);

            return Ok(new { message = "Token renovado." });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshTokenCookie = Request.Cookies["refresh_token"];
            if (refreshTokenCookie != null)
            {
                var rt = await _context.RefreshToken
                    .FirstOrDefaultAsync(r => r.Token == refreshTokenCookie);
                if (rt != null) { rt.Revocado = true; await _context.SaveChangesAsync(); }
            }
            await _audit.LogAsync("LOGOUT", "Usuario", null);
            ClearAuthCookies();
            return Ok(new { message = "Sesión cerrada." });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? "0");
            var usuario = await _context.Usuario.FindAsync(userId);
            if (usuario == null) return NotFound();

            var suscripcionActiva = await _context.Suscripcion
                .Include(s => s.Plan)
                .Where(s => s.UsuarioId == userId && (s.Estado == "activa" || s.Estado == "gratis_admin"))
                .OrderByDescending(s => s.FechaInicio)
                .FirstOrDefaultAsync();

            return Ok(new
            {
                id = usuario.Id,
                nombre = usuario.Nombre,
                email = usuario.Email,
                rol = usuario.Rol,
                isActive = usuario.IsActive,
                plan = suscripcionActiva?.Plan == null ? null : new
                {
                    nombre = suscripcionActiva.Plan.Nombre,
                    limiteBytes = suscripcionActiva.Plan.LimiteAlmacenamientoBytes,
                    usadoBytes = suscripcionActiva.AlmacenamientoUsadoBytes,
                    esGratis = suscripcionActiva.EsGratisAdminGranted
                }
            });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? "0");
            var usuario = await _context.Usuario.FindAsync(userId);
            if (usuario == null) return NotFound();
            if (!BCrypt.Net.BCrypt.Verify(req.PasswordActual, usuario.PasswordHash))
                return BadRequest(new { message = "Contraseña actual incorrecta." });
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NuevoPassword, workFactor: 12);
            await _context.SaveChangesAsync();
            await _audit.LogAsync("CAMBIO_CONTRASEÑA", "Usuario", userId.ToString());
            return Ok(new { message = "Contraseña actualizada." });
        }

        // Endpoint de primer arranque: crea el superadmin si no existe ninguno
        [HttpPost("setup")]
        [AllowAnonymous]
        public async Task<IActionResult> Setup([FromBody] SetupRequest req)
        {
            if (await _context.Usuario.AnyAsync(u => u.Rol == "superadmin"))
                return BadRequest(new { message = "El sistema ya está configurado. Este endpoint está deshabilitado." });

            var planGratuito = await _context.Plan.FirstOrDefaultAsync(p => p.EsGratuito && p.IsActive);

            var superAdmin = new Usuario
            {
                Email = req.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password, workFactor: 12),
                Nombre = req.Nombre,
                Rol = "superadmin",
                OrganizacionId = 1,
                IsActive = true
            };
            _context.Usuario.Add(superAdmin);
            await _context.SaveChangesAsync();

            if (planGratuito != null)
            {
                _context.Suscripcion.Add(new Suscripcion
                {
                    UsuarioId = superAdmin.Id,
                    PlanId = planGratuito.Id,
                    Estado = "gratis_admin",
                    EsGratisAdminGranted = true,
                    FechaInicio = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = $"SuperAdmin '{req.Nombre}' creado exitosamente. Ya puedes iniciar sesión." });
        }
    }

    public record LoginRequest(string Email, string Password);
    public record RegisterRequest(string Email, string Password, string Nombre, int OrganizacionId, string? Ciudad = null, string? Pais = null, string? Cargo = null, string? Empresa = null);
    public record ChangePasswordRequest(string PasswordActual, string NuevoPassword);
    public record SetupRequest(string Email, string Password, string Nombre);
}

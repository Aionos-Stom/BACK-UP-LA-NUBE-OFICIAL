using BackUp.Domainn.Entities;
using BackUp.Persistence.Context;
using System.Security.Claims;
using System.Text.Json;

namespace BackUp.API.Services
{
    public interface IAuditService
    {
        Task LogAsync(string accion, string entidad, string? entidadId = null, object? datosAnteriores = null, object? datosNuevos = null);
    }

    public class AuditService : IAuditService
    {
        private readonly BackUpDbContext _context;
        private readonly IHttpContextAccessor _http;

        public AuditService(BackUpDbContext context, IHttpContextAccessor http)
        {
            _context = context;
            _http = http;
        }

        public async Task LogAsync(string accion, string entidad, string? entidadId = null, object? datosAnteriores = null, object? datosNuevos = null)
        {
            var ctx = _http.HttpContext;
            var ua = ctx?.Request.Headers["User-Agent"].ToString();
            var ip = ObtenerIpReal(ctx);

            var log = new AuditLog
            {
                Accion = accion,
                Entidad = entidad,
                EntidadId = entidadId,
                DatosAnteriores = datosAnteriores != null ? JsonSerializer.Serialize(datosAnteriores) : null,
                DatosNuevos = datosNuevos != null ? JsonSerializer.Serialize(datosNuevos) : null,
                IpAddress = ip,
                UserAgent = ua,
                Navegador = DetectarNavegador(ua),
                SistemaOperativo = DetectarSO(ua),
                Dispositivo = DetectarDispositivo(ua),
                DescripcionLegible = GenerarDescripcion(accion, entidad, entidadId),
                UsuarioId = int.TryParse(ctx?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? ctx?.User.FindFirstValue("sub"), out var uid) ? uid : (int?)null,
                Email = ctx?.User.FindFirstValue(ClaimTypes.Email) ?? ctx?.User.FindFirstValue("email"),
                CreadoEn = DateTime.UtcNow
            };
            _context.AuditLog.Add(log);
            await _context.SaveChangesAsync();
        }

        private static string ObtenerIpReal(HttpContext? ctx)
        {
            if (ctx == null) return "Desconocida";
            var forwarded = ctx.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwarded))
                return forwarded.Split(',')[0].Trim();
            var realIp = ctx.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp)) return realIp;
            return ctx.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
        }

        private static string DetectarNavegador(string? ua)
        {
            if (string.IsNullOrEmpty(ua)) return "Desconocido";
            if (ua.Contains("Edg/") || ua.Contains("EdgA/")) return "Microsoft Edge";
            if (ua.Contains("OPR/") || ua.Contains("Opera")) return "Opera";
            if (ua.Contains("YaBrowser/")) return "Yandex Browser";
            if (ua.Contains("SamsungBrowser/")) return "Samsung Internet";
            if (ua.Contains("Chrome/") && !ua.Contains("Chromium")) return "Google Chrome";
            if (ua.Contains("Chromium/")) return "Chromium";
            if (ua.Contains("Firefox/")) return "Mozilla Firefox";
            if (ua.Contains("Safari/") && ua.Contains("Version/") && !ua.Contains("Chrome")) return "Safari";
            if (ua.Contains("MSIE") || ua.Contains("Trident/")) return "Internet Explorer";
            return "Otro navegador";
        }

        private static string DetectarSO(string? ua)
        {
            if (string.IsNullOrEmpty(ua)) return "Desconocido";
            if (ua.Contains("Windows NT 10.0")) return "Windows 10/11";
            if (ua.Contains("Windows NT 6.3")) return "Windows 8.1";
            if (ua.Contains("Windows NT 6.2")) return "Windows 8";
            if (ua.Contains("Windows NT 6.1")) return "Windows 7";
            if (ua.Contains("Windows")) return "Windows";
            if (ua.Contains("Android")) return "Android";
            if (ua.Contains("iPhone")) return "iPhone (iOS)";
            if (ua.Contains("iPad")) return "iPad (iPadOS)";
            if (ua.Contains("Mac OS X")) return "macOS";
            if (ua.Contains("CrOS")) return "Chrome OS";
            if (ua.Contains("Linux")) return "Linux";
            return "Otro SO";
        }

        private static string DetectarDispositivo(string? ua)
        {
            if (string.IsNullOrEmpty(ua)) return "Desconocido";
            if (ua.Contains("Mobile") || ua.Contains("iPhone") || ua.Contains("Android") && !ua.Contains("Tablet"))
                return "Móvil";
            if (ua.Contains("Tablet") || ua.Contains("iPad")) return "Tablet";
            return "Escritorio";
        }

        private static string GenerarDescripcion(string accion, string entidad, string? entidadId)
        {
            var id = entidadId != null ? $" (ID: {entidadId})" : "";
            return accion switch
            {
                "LOGIN" => "Inicio de sesión exitoso en el sistema",
                "LOGOUT" => "Cierre de sesión del sistema",
                "REGISTER" => "Registro de nueva cuenta de usuario",
                "CAMBIO_ROL" => $"Se modificó el rol del usuario{id}",
                "OTORGAR_PLAN_GRATIS" => $"Se otorgó un plan gratuito al usuario{id}",
                "REVOCAR_PLAN_GRATIS" => $"Se revocó el plan gratuito del usuario{id}",
                "ACTIVAR_USUARIO" => $"Se activó la cuenta del usuario{id}",
                "DESACTIVAR_USUARIO" => $"Se desactivó la cuenta del usuario{id}",
                "FORZAR_LOGOUT" => $"Se forzó el cierre de todas las sesiones del usuario{id}",
                "ACTUALIZAR_PERFIL" => "El usuario actualizó sus datos de perfil",
                "SUBIR_FOTO" => "El usuario actualizó su foto de perfil",
                "CAMBIO_CONTRASEÑA" => "El usuario cambió su contraseña de acceso",
                "REVOCAR_SESION" => "Se revocó una sesión activa del usuario",
                "REVOCAR_TODAS_SESIONES" => "Se revocaron todas las sesiones activas del usuario",
                "CREAR_USUARIO_ADMIN" => $"Un administrador creó una nueva cuenta de usuario{id}",
                "SUSCRIPCION" => $"El usuario realizó una suscripción a un plan{id}",
                _ => $"Acción '{accion}' realizada sobre {entidad}{id}"
            };
        }
    }
}

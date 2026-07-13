
using BackUp.Domain.Entities.Bac;
using BackUp.Domainn.Base;

namespace BackUp.Domainn.Entities.Users
{
    public sealed class Usuario : UsuarioBase
    {
        public int Id { get; set; }
        public int OrganizacionId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Rol { get; set; } = "usuario"; // 'superadmin', 'admin', 'auditor', 'usuario'
        public bool MfaHabilitado { get; set; } = false;
        public string? MfaSecret { get; set; }
        public DateTime? LastLogin { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;

        // Perfil extendido
        public string? FotoPerfil { get; set; }
        public string? Bio { get; set; }
        public string? Ciudad { get; set; }
        public string? Pais { get; set; }
        public string? Cargo { get; set; }
        public string? Empresa { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? LinkedIn { get; set; }

        // Navigation properties
        public Organizacion? Organizacion { get; set; }
        public List<Recuperacion> Recuperaciones { get; set; } = new();
        public List<Alerta> Alertas { get; set; } = new();
        public List<Sesion> Sesiones { get; set; } = new();
        public List<RefreshToken> RefreshTokens { get; set; } = new();

        // Business methods
        public bool TienePermisoAdmin() => Rol == "admin" || Rol == "superadmin";
        public bool TienePermisoAuditor() => Rol == "auditor" || Rol == "admin" || Rol == "superadmin";
        public bool PuedeConfigurarPoliticas() => Rol == "admin" || Rol == "superadmin";
    }
}

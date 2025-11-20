
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
        public string Rol { get; set; } = "usuario"; // 'admin', 'usuario', 'auditor'
        public bool MfaHabilitado { get; set; } = false;
        public string? MfaSecret { get; set; }
        public DateTime? LastLogin { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Organizacion? Organizacion { get; set; }
        public List<Recuperacion> Recuperaciones { get; set; } = new();
        public List<Alerta> Alertas { get; set; } = new();
        public List<Sesion> Sesiones { get; set; } = new();

        // Business methods
        public bool TienePermisoAdmin() => Rol == "admin";
        public bool TienePermisoAuditor() => Rol == "auditor" || Rol == "admin";
        public bool PuedeConfigurarPoliticas() => Rol == "admin";
    }
}

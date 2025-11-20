using BackUp.Domainn.Base;

namespace BackUp.Domainn.Entities.Users
{
    public sealed class Sesion 
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }

        // Navigation properties
        public Usuario? Usuario { get; set; }

        // Business methods
        public bool EstaExpirada() => DateTime.UtcNow > ExpiresAt;
        public bool EsValida() => !EstaExpirada();

        public void ExtenderExpiracion(TimeSpan extension)
        {
            ExpiresAt = ExpiresAt.Add(extension);
        }
    }
}

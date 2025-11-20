

namespace BackUp.Aplication.Dtos.Usuario
{
    public record SaveUsuarioDTO
    {
        public int OrganizacionId { get; init; }
        public string Email { get; init; }
        public string Password { get; init; }
        public string Nombre { get; init; }
        public string Rol { get; init; }
        public string? PhoneNumber { get; init; }
    }
}

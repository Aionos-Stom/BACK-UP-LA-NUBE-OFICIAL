namespace BackUp.Domainn.Entities.Users
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime Expiracion { get; set; }
        public bool Revocado { get; set; } = false;
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
        public string? ReemplazadoPor { get; set; }
        public Usuario? Usuario { get; set; }
        public bool EstaActivo => !Revocado && DateTime.UtcNow < Expiracion;
    }
}

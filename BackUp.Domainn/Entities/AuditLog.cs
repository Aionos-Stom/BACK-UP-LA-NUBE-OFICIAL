namespace BackUp.Domainn.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int? UsuarioId { get; set; }
        public string Accion { get; set; } = string.Empty;
        public string Entidad { get; set; } = string.Empty;
        public string? EntidadId { get; set; }
        public string? DatosAnteriores { get; set; }
        public string? DatosNuevos { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Navegador { get; set; }
        public string? SistemaOperativo { get; set; }
        public string? Dispositivo { get; set; }
        public string? DescripcionLegible { get; set; }
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
        public string? Email { get; set; }
    }
}

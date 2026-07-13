namespace BackUp.Domain.Entities.Bac
{
    public class Pago
    {
        public int Id { get; set; }
        public int SuscripcionId { get; set; }
        public decimal Monto { get; set; }
        public string Moneda { get; set; } = "USD";
        public string Estado { get; set; } = "pendiente"; // pendiente, completado, fallado, reembolsado
        public string MetodoPago { get; set; } = string.Empty;
        public string? ReferenciaExterna { get; set; }
        public DateTime FechaPago { get; set; } = DateTime.UtcNow;
        public string? Descripcion { get; set; }
        public Suscripcion? Suscripcion { get; set; }
    }
}

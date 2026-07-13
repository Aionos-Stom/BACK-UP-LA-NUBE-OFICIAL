namespace BackUp.Domain.Entities.Bac
{
    public class Plan
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal PrecioMensual { get; set; }
        public long LimiteAlmacenamientoBytes { get; set; }
        public int MaxJobsConcurrentes { get; set; }
        public int MaxPoliticas { get; set; }
        public bool BackupAutomatico { get; set; }
        public bool SoportePrioritario { get; set; }
        public bool EsGratuito { get; set; }
        public bool IsActive { get; set; } = true;
        public List<Suscripcion> Suscripciones { get; set; } = new();
    }
}

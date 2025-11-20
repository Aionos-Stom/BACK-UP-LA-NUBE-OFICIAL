

using BackUp.Domainn.Base;

namespace BackUp.Domain.Entities.Bac
{
    public sealed class JobBackup 
    {
        public int Id { get; set; }
        public int PoliticaId { get; set; }
        public int CloudStorageId { get; set; }
        public string Estado { get; set; } = "programado"; // 'programado', 'ejecutando', 'completado', 'fallado'
        public DateTime? FechaProgramada { get; set; }
        public DateTime? FechaEjecucion { get; set; }
        public DateTime? FechaCompletado { get; set; }
        public long? TamanoBytes { get; set; }
        public int? DuracionSegundos { get; set; }
        public string? SourceData { get; set; }
        public string? ErrorMessage { get; set; }

        // Navigation properties
      public PoliticaBackup? Politica { get; set; }
        public CloudStorage? CloudStorage { get; set; }
        public List<VerificacionIntegridad> VerificacionesIntegridad { get; set; } = new();
        public List<Recuperacion> Recuperaciones { get; set; } = new();
        public List<Alerta> Alertas { get; set; } = new();

        // Business methods
        public bool EstaCompletado() => Estado == "completado";
        public bool EstaFallido() => Estado == "fallado";
        public bool EstaEnEjecucion() => Estado == "ejecutando";

        public void MarcarComoCompletado(long tamanoBytes, int duracionSegundos)
        {
            Estado = "completado";
            FechaCompletado = DateTime.UtcNow;
            TamanoBytes = tamanoBytes;
            DuracionSegundos = duracionSegundos;
        }

        public void MarcarComoFallido(string errorMessage)
        {
            Estado = "fallado";
            ErrorMessage = errorMessage;
            FechaCompletado = DateTime.UtcNow;
        }
    }
}


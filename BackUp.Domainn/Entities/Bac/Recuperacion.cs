
using BackUp.Domainn.Base;
using BackUp.Domainn.Entities.Users;

namespace BackUp.Domain.Entities.Bac
{
    public sealed class Recuperacion
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int JobId { get; set; }
        public string? TipoRecuperacion { get; set; } // 'archivo', 'base_datos', 'maquina_virtual'
        public DateTime? PuntoTiempo { get; set; }
        public string? InputPath { get; set; }
        public string? Estado { get; set; } // 'pendiente', 'en_progreso', 'completado', 'fallado'
        public bool IsSimulacion { get; set; } = false;
        public DateTime? CompletedAt { get; set; }

        // Navigation properties
        public Usuario? Usuario { get; set; }
        public JobBackup? JobBackup { get; set; }

        // Business methods
        public bool EsRecuperacionGranular() =>
            TipoRecuperacion == "archivo" || TipoRecuperacion == "base_datos";

        public bool EsPointInTimeRecovery() => PuntoTiempo.HasValue;

        public void MarcarComoCompletado()
        {
            Estado = "completado";
            CompletedAt = DateTime.UtcNow;
        }
    }
}

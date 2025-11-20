

using BackUp.Domainn.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackUp.Domain.Entities.Bac
{
    public sealed class PoliticaBackup 
    {
        public int Id { get; set; }
        public int OrganizacionId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Frecuencia { get; set; } = string.Empty; // 'horaria', 'diaria', 'semanal', 'mensual'
        public string TipoBackup { get; set; } = string.Empty; // 'completo', 'incremental', 'diferencial'
        public int RetencionDias { get; set; }
        public int RpoMinutes { get; set; }
        public int RtoMinutes { get; set; }
        public string? VentanaEjecucion { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Organizacion? Organizacion { get; set; }
        public List<JobBackup> JobsBackup { get; set; } = new();

        // Business methods
        public bool EstaEnVentanaEjecucion()
        {
            // Lógica para verificar si está en ventana de ejecución
            return true; // Implementar lógica real
        }

        public TimeSpan CalcularProximaEjecucion()
        {
            // Lógica para calcular próxima ejecución basada en frecuencia
            return TimeSpan.FromHours(1); // Implementar lógica real
        }
    }
}


using BackUp.Domainn.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackUp.Domain.Entities.Bac
{
    public sealed class VerificacionIntegridad 
    {
        public int Id { get; set; }
        [Required]
        [Column("job_id")]
        public int job_id { get; set; } // Nombre correcto de la columna
        public string? ChecksumSha256 { get; set; }
        public bool? Resultado { get; set; }
        public DateTime FechaVerificacion { get; set; } = DateTime.UtcNow;
        public string? Detalles { get; set; }
        public decimal? IntegrityScore { get; set; }

        // Navigation properties
        public JobBackup? JobBackup { get; set; }

        // Business methods
        public bool EsIntegro() => Resultado == true;

        public void CalcularIntegrityScore()
        {
            // Lógica para calcular score de integridad basado en múltiples factores
            IntegrityScore = Resultado == true ? 100.00m : 0.00m;
        }
    }
}

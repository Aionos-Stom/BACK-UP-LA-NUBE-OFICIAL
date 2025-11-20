
using BackUp.Domainn.Base;

namespace BackUp.Domain.Entities.Bac
{
    public sealed class CloudStorage 
    {
        public int Id { get; set; }
        public int OrganizacionId { get; set; }
        public string Proveedor { get; set; } = string.Empty; // 'aws', 'azure', 'gcp'
        public string? Configuration { get; set; }
        public string? EndpointUrl { get; set; }
        public string TierActual { get; set; } = string.Empty; // 'frecuente', 'infrecuente', 'archivo'
        public decimal CostoMensual { get; set; } = 0.00m;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Organizacion? Organizacion { get; set; }
        public List<JobBackup> JobsBackup { get; set; } = new();
    }
}

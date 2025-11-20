using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Aplication.Dtos.CloudStorage

{
    public record SaveCloudStorageDTO
    {
        public int OrganizacionId { get; init; }
        public string Proveedor { get; init; } = string.Empty; // 'aws', 'azure', 'gcp'
        public string? Configuration { get; init; }
        public string? EndpointUrl { get; init; }
        public string TierActual { get; init; } = string.Empty; // 'frecuente', 'infrecuente', 'archivo'
        public decimal CostoMensual { get; init; } = 0.00m;
        public bool IsActive { get; init; } = true;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Aplication.Dtos.CloudStorage
{
    public record ObtenerCloudStorageDTO
    {
        public int Id { get; init; }
        public int OrganizacionId { get; init; }
        public string Proveedor { get; init; }
        public string? EndpointUrl { get; init; }
        public string TierActual { get; init; }
        public decimal CostoMensual { get; init; }
        public bool IsActive { get; init; }
        public DateTime FechaCreacion { get; init; }
    }
}

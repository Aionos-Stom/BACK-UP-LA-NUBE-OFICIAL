

using BackUp.Aplication.Dtos.organizacion;
using BackUp.Domain.Base;

namespace BackUp.Aplication.Interfaces.IService
{
    public interface IOrganizacionService
    {
        Task<OperationResult> ObtenerPorIdAsync(int id);
        Task<OperationResult> ObtenerTodosAsync();
        Task<OperationResult> AgregarAsync(SaveOrganizacionDTO saveOrganizacion);
        Task<OperationResult> ActualizarAsync(UpdateOrganizacionDTO updateOrganizacion);
        Task<OperationResult> EliminarAsync(RemoveOrganizacionDTO removeOrganizacion);
    }
}

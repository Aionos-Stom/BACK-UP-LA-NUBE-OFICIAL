
using BackUp.Aplication.Dtos.organizacion;
using BackUp.Domain.Base;
namespace BackUp.Aplication.Interfaces.Repositorios
{
    public interface IOrganizacionRepository
    {
        Task<OperationResult> ObtenerPorIdAsync(int id);
        Task<OperationResult> ObtenerTodosAsync();
        Task<OperationResult> AgregarAsync(SaveOrganizacionDTO saveOrganizacion);
        Task<OperationResult> ActualizarAsync(UpdateOrganizacionDTO updateOrganizacion);
        Task<OperationResult> EliminarAsync(RemoveOrganizacionDTO removeOrganizacion);
    }

}

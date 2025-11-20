
using BackUp.Aplication.Dtos.CloudStorage;
using BackUp.Aplication.Interfaces.Base;
using BackUp.Domain.Base;
using BackUp.Domain.Entities.Bac;

namespace BackUp.Aplication.Interfaces.Repositorios
{
    public interface ICloudStorageRepository
    {
        Task<OperationResult> ObtenerPorIdAsync(int id);
        Task<OperationResult> ObtenerTodosAsync();
        Task<OperationResult> AgregarAsync(SaveCloudStorageDTO saveCloudStorage);
        Task<OperationResult> ActualizarAsync(UpdateCloudStorageDTO updateCloudStorage);
        Task<OperationResult> EliminarAsync(RemoveCloudStorageDTO removeCloudStorage);
        Task<OperationResult> ObtenerPorOrganizacionAsync(int organizacionId);
    }

}

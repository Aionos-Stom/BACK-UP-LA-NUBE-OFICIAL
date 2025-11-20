using BackUp.Aplication.Dtos.CloudStorage;
using BackUp.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Aplication.Interfaces.IService
{
    public interface ICloudStorageService
    {
        Task<OperationResult> ObtenerPorIdAsync(int id);
        Task<OperationResult> ObtenerTodosAsync();
        Task<OperationResult> AgregarAsync(SaveCloudStorageDTO saveCloudStorage);
        Task<OperationResult> ActualizarAsync(UpdateCloudStorageDTO updateCloudStorage);
        Task<OperationResult> EliminarAsync(RemoveCloudStorageDTO removeCloudStorage);
        Task<OperationResult> ObtenerPorOrganizacionAsync(int organizacionId);
    }
}

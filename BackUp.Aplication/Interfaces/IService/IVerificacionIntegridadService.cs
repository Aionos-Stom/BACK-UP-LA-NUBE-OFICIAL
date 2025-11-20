using BackUp.Aplication.Dtos.VerificacionIntegridad;
using BackUp.Domain.Base;

namespace BackUp.Aplication.Interfaces.IService
{
    public interface IVerificacionIntegridadService
    {
        Task<OperationResult> ObtenerPorIdAsync(int id);
        Task<OperationResult> ObtenerTodosAsync();
        Task<OperationResult> AgregarAsync(SaveVerificacionIntegridadDTO saveVerificacionIntegridad);
        Task<OperationResult> EliminarAsync(RemoveVerificacionIntegridadDTO removeVerificacionIntegridad);
        Task<OperationResult> VerificarIntegridadAsync(int jobId);
    }
}

using BackUp.Aplication.Dtos.VerificacionIntegridad;
using BackUp.Aplication.Interfaces.Base;
using BackUp.Domain.Base;
using BackUp.Domain.Entities.Bac;

namespace BackUp.Aplication.Interfaces.Repositorios
{
    public interface IVerificacionIntegridadRepository
    {
        Task<OperationResult> ObtenerPorIdAsync(int id);
        Task<OperationResult> ObtenerTodosAsync();
        Task<OperationResult> AgregarAsync(SaveVerificacionIntegridadDTO saveVerificacionIntegridad);
        Task<OperationResult> EliminarAsync(RemoveVerificacionIntegridadDTO removeVerificacionIntegridad);
        Task<OperationResult> ObtenerPorJobAsync(int jobId);
        Task<ObtenerVerificacionIntegridadDTO> ObtenerUltimaPorJobAsync(int jobId);
    }
}

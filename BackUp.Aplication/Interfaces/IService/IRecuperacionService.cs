

using BackUp.Aplication.Dtos.Recuperacion;
using BackUp.Domain.Base;

namespace BackUp.Aplication.Interfaces.IService
{
    public interface IRecuperacionService
    {
        Task<OperationResult> ObtenerPorIdAsync(int id);
        Task<OperationResult> ObtenerTodosAsync();
        Task<OperationResult> AgregarAsync(SaveRecuperacionDTO saveRecuperacion);
        Task<OperationResult> ActualizarAsync(UpdateRecuperacionDTO updateRecuperacion);
        Task<OperationResult> EliminarAsync(RemoveRecuperacionDTO removeRecuperacion);
        Task<OperationResult> EjecutarRecuperacionAsync(int recuperacionId);
        Task<OperationResult> SimularRecuperacionAsync(int recuperacionId);
    }
}

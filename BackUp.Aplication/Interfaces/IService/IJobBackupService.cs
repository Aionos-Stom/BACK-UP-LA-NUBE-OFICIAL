
using BackUp.Aplication.Dtos.JobBackup;
using BackUp.Domain.Base;
using BackUp.Aplication.Interfaces.Base;
using Microsoft.EntityFrameworkCore; 

namespace BackUp.Aplication.Interfaces.IService
{
    public interface IJobBackupService
    {
        Task<OperationResult> ObtenerPorIdAsync(int id);
        Task<OperationResult> ObtenerTodosAsync();
        Task<OperationResult> AgregarAsync(SaveJobBackupDTO saveJobBackup);
        Task<OperationResult> ActualizarAsync(UpdateJobBackupDTO updateJobBackup);
        Task<OperationResult> EliminarAsync(RemoveJobBackupDTO removeJobBackup);
        Task<OperationResult> EjecutarJobAsync(int jobId);
        Task<OperationResult> ObtenerJobsProgramadosAsync();
    }
}

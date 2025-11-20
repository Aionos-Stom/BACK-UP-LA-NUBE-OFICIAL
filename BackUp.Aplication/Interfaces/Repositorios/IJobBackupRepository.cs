
using BackUp.Aplication.Dtos.JobBackup;

using BackUp.Domain.Base;


namespace BackUp.Aplication.Interfaces.Repositorios
{
    public interface IJobBackupRepository
    {
        Task<OperationResult> ObtenerPorIdAsync(int id);
        Task<OperationResult> ObtenerTodosAsync(int id);
        Task<OperationResult> AgregarAsync(SaveJobBackupDTO saveJobBackup);
        Task<OperationResult> ActualizarAsync(UpdateJobBackupDTO updateJobBackup);
        Task<OperationResult> EliminarAsync(RemoveJobBackupDTO removeJobBackup);
        Task<OperationResult> ObtenerPorEstadoAsync(string estado);
        Task<OperationResult> ObtenerPorPoliticaAsync(int politicaId);
        Task<OperationResult> ObtenerEstadisticasDashboardAsync();
        Task<OperationResult> ObtenerBackupsRecientesAsync(int cantidad = 10);
    }

}

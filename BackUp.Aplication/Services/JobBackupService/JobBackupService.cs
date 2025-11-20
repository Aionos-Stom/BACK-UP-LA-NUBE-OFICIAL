using BackUp.Aplication.Dtos.JobBackup;
using BackUp.Aplication.Interfaces.Base;
using BackUp.Aplication.Interfaces.IService;
using BackUp.Aplication.Interfaces.Repositorios;
using BackUp.Domain.Base;
using BackUp.Domain.Entities.Bac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace BackUp.Aplication.Services.JobBackupService
{
    public sealed class JobBackupService : IJobBackupService
    {
        private readonly IJobBackupRepository _jobBackupRepository;

        public JobBackupService(IJobBackupRepository jobBackupRepository)
        {
            _jobBackupRepository = jobBackupRepository;
        }

        public async Task<OperationResult> ObtenerPorIdAsync(int id)
        {
            if (id <= 0)
                return OperationResult.Failure("ID de job inválido");

            return await _jobBackupRepository.ObtenerPorIdAsync(id);
        }

        public async Task<OperationResult> ObtenerTodosAsync()
        {
            return await _jobBackupRepository.ObtenerTodosAsync(0); // El parámetro no se usa
        }

        public async Task<OperationResult> AgregarAsync(SaveJobBackupDTO saveJobBackup)
        {
            if (saveJobBackup.PoliticaId <= 0)
                return OperationResult.Failure("Política ID inválido");

            if (saveJobBackup.CloudStorageId <= 0)
                return OperationResult.Failure("Cloud Storage ID inválido");

            if (saveJobBackup.FechaProgramada.HasValue && saveJobBackup.FechaProgramada.Value < DateTime.UtcNow)
                return OperationResult.Failure("La fecha programada no puede ser en el pasado");

            return await _jobBackupRepository.AgregarAsync(saveJobBackup);
        }

        public async Task<OperationResult> ActualizarAsync(UpdateJobBackupDTO updateJobBackup)
        {
            if (updateJobBackup.Id <= 0)
                return OperationResult.Failure("ID de job inválido");

            var estadosValidos = new[] { "programado", "ejecutando", "completado", "fallado" };
            if (!estadosValidos.Contains(updateJobBackup.Estado?.ToLower()))
                return OperationResult.Failure("Estado inválido");

            return await _jobBackupRepository.ActualizarAsync(updateJobBackup);
        }

        public async Task<OperationResult> EliminarAsync(RemoveJobBackupDTO removeJobBackup)
        {
            if (removeJobBackup.Id <= 0)
                return OperationResult.Failure("ID de job inválido");

            return await _jobBackupRepository.EliminarAsync(removeJobBackup);
        }

        public async Task<OperationResult> EjecutarJobAsync(int jobId)
        {
            if (jobId <= 0)
                return OperationResult.Failure("ID de job inválido");

            // Lógica para ejecutar el job
            var updateDto = new UpdateJobBackupDTO
            {
                Id = jobId,
                Estado = "ejecutando",
                FechaEjecucion = DateTime.UtcNow
            };

            return await _jobBackupRepository.ActualizarAsync(updateDto);
        }

        public async Task<OperationResult> ObtenerJobsProgramadosAsync()
        {
            return await _jobBackupRepository.ObtenerPorEstadoAsync("programado");
        }
    }
}


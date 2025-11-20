using BackUp.Aplication.Dtos.JobBackup;
using BackUp.Aplication.Interfaces.Repositorios;
using BackUp.Domain.Base;
using BackUp.Domain.Entities.Bac;
using BackUp.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Linq.Expressions;

namespace BackUp.Persistence.Repositories
{
    public class JobBackupRepository : IJobBackupRepository
    {
        private readonly BackUpDbContext _context;

        public JobBackupRepository(BackUpDbContext context)
        {
            _context = context;
        }

        public async Task<OperationResult> ObtenerPorIdAsync(int id)
        {
            try
            {
                var job = await _context.JobBackup
                    .Include(j => j.Politica)
                    .Include(j => j.CloudStorage)
                    .FirstOrDefaultAsync(j => j.Id == id);

                if (job == null)
                    return OperationResult.Failure("Job de backup no encontrado");

                var dto = new ObtenerJobBackupDTO
                {
                    Id = job.Id,
                    PoliticaId = job.PoliticaId,
                    CloudStorageId = job.CloudStorageId,
                    Estado = job.Estado,
                    FechaProgramada = job.FechaProgramada,
                    FechaEjecucion = job.FechaEjecucion,
                    FechaCompletado = job.FechaCompletado,
                    TamanoBytes = job.TamanoBytes,
                    DuracionSegundos = job.DuracionSegundos,
                    SourceData = job.SourceData,
                    ErrorMessage = job.ErrorMessage,
                 
                };

                return OperationResult.Success(dto);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al obtener job: {ex.Message}");
            }
        }

        public async Task<OperationResult> ObtenerTodosAsync(int id)
        {
            try
            {
                var jobs = await _context.JobBackup
                    .Include(j => j.Politica)
                    .Include(j => j.CloudStorage)
                    .Select(j => new ObtenerJobBackupDTO
                    {
                        Id = j.Id,
                        PoliticaId = j.PoliticaId,
                        CloudStorageId = j.CloudStorageId,
                        Estado = j.Estado,
                        FechaProgramada = j.FechaProgramada,
                        FechaEjecucion = j.FechaEjecucion,
                        FechaCompletado = j.FechaCompletado,
                        TamanoBytes = j.TamanoBytes,
                        DuracionSegundos = j.DuracionSegundos,
                        SourceData = j.SourceData,
                        ErrorMessage = j.ErrorMessage,
                        
                    })
                    .ToListAsync();

                return OperationResult.Success(jobs);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al obtener jobs: {ex.Message}");
            }
        }

        public async Task<OperationResult> AgregarAsync(SaveJobBackupDTO saveJobBackup)
        {
            try
            {
                var job = new Domain.Entities.Bac.JobBackup
                {
                    PoliticaId = saveJobBackup.PoliticaId,
                    CloudStorageId = saveJobBackup.CloudStorageId,
                    Estado = "programado",
                    FechaProgramada = saveJobBackup.FechaProgramada,
                    SourceData = saveJobBackup.SourceData,
                    
                };

                _context.JobBackup.Add(job);
                await _context.SaveChangesAsync();

                return OperationResult.Success(job.Id);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al crear job: {ex.Message}");
            }
        }

        public async Task<OperationResult> ActualizarAsync(UpdateJobBackupDTO updateJobBackup)
        {
            try
            {
                var job = await _context.JobBackup
                    .FirstOrDefaultAsync(j => j.Id == updateJobBackup.Id);

                if (job == null)
                    return OperationResult.Failure("Job de backup no encontrado");

                job.Estado = updateJobBackup.Estado;
                job.FechaEjecucion = updateJobBackup.FechaEjecucion;
                job.FechaCompletado = updateJobBackup.FechaCompletado;
                job.TamanoBytes = updateJobBackup.TamanoBytes;
                job.DuracionSegundos = updateJobBackup.DuracionSegundos;
                job.ErrorMessage = updateJobBackup.ErrorMessage;

                await _context.SaveChangesAsync();

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al actualizar job: {ex.Message}");
            }
        }

        public async Task<OperationResult> EliminarAsync(RemoveJobBackupDTO removeJobBackup)
        {
            try
            {
                var job = await _context.JobBackup
                    .FirstOrDefaultAsync(j => j.Id == removeJobBackup.Id);

                if (job == null)
                    return OperationResult.Failure("Job de backup no encontrado");

                _context.JobBackup.Remove(job);
                await _context.SaveChangesAsync();

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al eliminar job: {ex.Message}");
            }
        }

        public async Task<OperationResult> ObtenerPorEstadoAsync(string estado)
        {
            try
            {
                var jobs = await _context.JobBackup
                    .Where(j => j.Estado == estado)
                    .Include(j => j.Politica)
                    .Include(j => j.CloudStorage)
                    .Select(j => new ObtenerJobBackupDTO
                    {
                        Id = j.Id,
                        PoliticaId = j.PoliticaId,
                        CloudStorageId = j.CloudStorageId,
                        Estado = j.Estado,
                        FechaProgramada = j.FechaProgramada,
                        FechaEjecucion = j.FechaEjecucion,
                        FechaCompletado = j.FechaCompletado,
                        TamanoBytes = j.TamanoBytes,
                        DuracionSegundos = j.DuracionSegundos,
                        SourceData = j.SourceData,
                        ErrorMessage = j.ErrorMessage,
                       
                    })
                    .ToListAsync();

                return OperationResult.Success(jobs);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al obtener jobs por estado: {ex.Message}");
            }
        }

        public async Task<OperationResult> ObtenerPorPoliticaAsync(int politicaId)
        {
            try
            {
                var jobs = await _context.JobBackup
                    .Where(j => j.PoliticaId == politicaId)
                    .Include(j => j.Politica)
                    .Include(j => j.CloudStorage)                
                    .Select(j => new ObtenerJobBackupDTO
                    {
                        Id = j.Id,
                        PoliticaId = j.PoliticaId,
                        CloudStorageId = j.CloudStorageId,
                        Estado = j.Estado,
                        FechaProgramada = j.FechaProgramada,
                        FechaEjecucion = j.FechaEjecucion,
                        FechaCompletado = j.FechaCompletado,
                        TamanoBytes = j.TamanoBytes,
                        DuracionSegundos = j.DuracionSegundos,
                        SourceData = j.SourceData,
                        ErrorMessage = j.ErrorMessage,

                    })
                    .ToListAsync();

                return OperationResult.Success(jobs);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al obtener jobs por política: {ex.Message}");
            }
        }

        public async Task<OperationResult> ObtenerEstadisticasDashboardAsync()
        {
            // Implementar según necesidades del dashboard
            return OperationResult.Success();
        }

        public async Task<OperationResult> ObtenerBackupsRecientesAsync(int cantidad = 10)
        {
            try
            {
                var jobs = await _context.JobBackup
                    .Include(j => j.Politica)
                    .Include(j => j.CloudStorage)
                    .Take(cantidad)
                    .Select(j => new ObtenerJobBackupDTO
                    {
                        Id = j.Id,
                        PoliticaId = j.PoliticaId,
                        CloudStorageId = j.CloudStorageId,
                        Estado = j.Estado,
                        FechaProgramada = j.FechaProgramada,
                        FechaEjecucion = j.FechaEjecucion,
                        FechaCompletado = j.FechaCompletado,
                        TamanoBytes = j.TamanoBytes,
                        DuracionSegundos = j.DuracionSegundos,
                        SourceData = j.SourceData,
                        ErrorMessage = j.ErrorMessage,

                    })
                    .ToListAsync();

                return OperationResult.Success(jobs);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al obtener backups recientes: {ex.Message}");
            }
        }
    }
}

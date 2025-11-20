using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BackUp.Aplication.Dtos.Recuperacion;
using BackUp.Aplication.Interfaces.Base;
using BackUp.Aplication.Interfaces.IService;
using BackUp.Aplication.Interfaces.Repositorios;
using BackUp.Domain.Base;
using BackUp.Domain.Entities.Bac;

namespace BackUp.Aplication.Services.RecuperacionService
{
    public sealed class RecuperacionService :
          BaseService<Recuperacion, SaveRecuperacionDTO, UpdateRecuperacionDTO, RemoveRecuperacionDTO>, IRecuperacionService
    {
        private readonly ILogger<RecuperacionService> _logger;

        public RecuperacionService(
            ILogger<RecuperacionService> logger,
            IApplicationDbContext context) : base((DbContext)context, logger)
        {
            _logger = logger;
        }

        public async Task<OperationResult> EjecutarRecuperacionAsync(int recuperacionId)
        {
            try
            {
                var recuperacion = await _dbSet.FindAsync(recuperacionId);
                if (recuperacion == null)
                    return OperationResult.Failure("Recuperación no encontrada.");

                // Lógica para ejecutar la recuperación
                recuperacion.Estado = "completado";
                recuperacion.CompletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return OperationResult.Success("Recuperación ejecutada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al ejecutar recuperación");
                return OperationResult.Failure("Error al ejecutar recuperación.");
            }
        }

        public async Task<OperationResult> SimularRecuperacionAsync(int recuperacionId)
        {
            try
            {
                var recuperacion = await _dbSet.FindAsync(recuperacionId);
                if (recuperacion == null)
                    return OperationResult.Failure("Recuperación no encontrada.");

                recuperacion.IsSimulacion = true;
                recuperacion.Estado = "simulacion_completada";

                await _context.SaveChangesAsync();

                return OperationResult.Success("Simulación de recuperación completada.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en simulación de recuperación");
                return OperationResult.Failure("Error en simulación de recuperación.");
            }
        }

        protected override Recuperacion MapSaveDtoToEntity(SaveRecuperacionDTO dto)
        {
            return new Recuperacion
            {
                UsuarioId = dto.UsuarioId,
                JobId = dto.JobId,
                TipoRecuperacion = dto.TipoRecuperacion,
                PuntoTiempo = dto.PuntoTiempo,
                InputPath = dto.InputPath,
                IsSimulacion = dto.IsSimulacion,
                Estado = "pendiente"
            };
        }

        protected override Recuperacion MapUpdateDtoToEntity(UpdateRecuperacionDTO dto)
        {
            return new Recuperacion
            {
                Id = dto.Id,
                Estado = dto.Estado,
                CompletedAt = dto.CompletedAt
            };
        }

        protected override Recuperacion MapRemoveDtoToEntity(RemoveRecuperacionDTO dto)
        {
            return new Recuperacion { Id = dto.Id };
        }
    }
}
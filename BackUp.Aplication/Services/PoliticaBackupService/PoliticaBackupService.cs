using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BackUp.Aplication.Dtos.politicaBackup;
using BackUp.Aplication.Dtos.PoliticaBackup;
using BackUp.Aplication.Interfaces.Base;
using BackUp.Aplication.Interfaces.IService;
using BackUp.Aplication.Interfaces.Repositorios;
using BackUp.Domain.Base;
using BackUp.Domain.Entities.Bac;


namespace BackUp.Aplication.Services.PoliticaBackupService
{
    public sealed class PoliticaBackupService : IPoliticaBackupService
    {
        private readonly ILogger<PoliticaBackupService> _logger;
        private readonly IPoliticaBackupRepository _politicaBackupRepository;

        public PoliticaBackupService(
            ILogger<PoliticaBackupService> logger,
            IPoliticaBackupRepository politicaBackupRepository)
        {
            _logger = logger;
            _politicaBackupRepository = politicaBackupRepository;
        }

        public async Task<OperationResult> ObtenerPorIdAsync(int id)
        {
            try
            {
                return await _politicaBackupRepository.ObtenerPorIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener política de backup por ID: {Id}", id);
                return OperationResult.Failure($"Error al obtener política de backup: {ex.Message}");
            }
        }

        public async Task<OperationResult> ObtenerTodosAsync()
        {
            try
            {
                return await _politicaBackupRepository.ObtenerTodosAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las políticas de backup");
                return OperationResult.Failure($"Error al obtener políticas de backup: {ex.Message}");
            }
        }

        public async Task<OperationResult> CrearAsync(SavePoliticaBackupDTO savePoliticaBackup)
        {
            try
            {
                if (savePoliticaBackup == null)
                    return OperationResult.Failure("Los datos de la política de backup son requeridos");

                if (string.IsNullOrWhiteSpace(savePoliticaBackup.Nombre))
                    return OperationResult.Failure("El nombre de la política de backup es requerido");

                if (savePoliticaBackup.RetencionDias <= 0)
                    return OperationResult.Failure("Los días de retención deben ser mayores a 0");

                return await _politicaBackupRepository.AgregarAsync(savePoliticaBackup);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear política de backup");
                return OperationResult.Failure($"Error al crear política de backup: {ex.Message}");
            }
        }

        public async Task<OperationResult> ActualizarAsync(UpdatePoliticaBackupDTO updatePoliticaBackup)
        {
            try
            {
                if (updatePoliticaBackup == null)
                    return OperationResult.Failure("Los datos de actualización son requeridos");

                if (string.IsNullOrWhiteSpace(updatePoliticaBackup.Nombre))
                    return OperationResult.Failure("El nombre de la política de backup es requerido");

                if (updatePoliticaBackup.RetencionDias <= 0)
                    return OperationResult.Failure("Los días de retención deben ser mayores a 0");

                return await _politicaBackupRepository.ActualizarAsync(updatePoliticaBackup);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar política de backup: {Id}", updatePoliticaBackup?.Id);
                return OperationResult.Failure($"Error al actualizar política de backup: {ex.Message}");
            }
        }

        public async Task<OperationResult> EliminarAsync(RemovePoliticaBackupDTO removePoliticaBackup)
        {
            try
            {
                if (removePoliticaBackup == null)
                    return OperationResult.Failure("Los datos para eliminar son requeridos");

                return await _politicaBackupRepository.EliminarAsync(removePoliticaBackup);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar política de backup: {Id}", removePoliticaBackup?.Id);
                return OperationResult.Failure($"Error al eliminar política de backup: {ex.Message}");
            }
        }

        public async Task<OperationResult> ObtenerPorOrganizacionAsync(int organizacionId)
        {
            try
            {
                if (organizacionId <= 0)
                    return OperationResult.Failure("El ID de organización es inválido");

                return await _politicaBackupRepository.ObtenerPorOrganizacionAsync(organizacionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener políticas por organización: {OrganizacionId}", organizacionId);
                return OperationResult.Failure($"Error al obtener políticas por organización: {ex.Message}");
            }
        }

        public async Task<OperationResult> ObtenerActivasAsync()
        {
            try
            {
                return await _politicaBackupRepository.ObtenerActivasAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener políticas activas");
                return OperationResult.Failure($"Error al obtener políticas activas: {ex.Message}");
            }
        }

        public async Task<OperationResult> ObtenerPorTipoAsync(string tipoBackup)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tipoBackup))
                    return OperationResult.Failure("El tipo de backup es requerido");

                return await _politicaBackupRepository.ObtenerPorTipoAsync(tipoBackup);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener políticas por tipo: {TipoBackup}", tipoBackup);
                return OperationResult.Failure($"Error al obtener políticas por tipo: {ex.Message}");
            }
        }

        public async Task<OperationResult> CambiarEstadoAsync(int id, bool activo)
        {
            try
            {
                var resultado = await _politicaBackupRepository.ObtenerPorIdAsync(id);
                if (!resultado.IsSuccess)
                    return resultado;

                var politica = resultado.Data as ObtenerPoliticaBackupDTO;
                if (politica == null)
                    return OperationResult.Failure("Política de backup no encontrada");

                var updateDto = new UpdatePoliticaBackupDTO
                {
                    Id = id,
                    Nombre = politica.Nombre,
                    Frecuencia = politica.Frecuencia,
                    TipoBackup = politica.TipoBackup,
                    RetencionDias = politica.RetencionDias,
                    RpoMinutes = politica.RpoMinutes,
                    RtoMinutes = politica.RtoMinutes,
                    VentanaEjecucion = politica.VentanaEjecucion,
                    IsActive = activo
                };

                return await _politicaBackupRepository.ActualizarAsync(updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado de política de backup: {Id}", id);
                return OperationResult.Failure($"Error al cambiar estado de política de backup: {ex.Message}");
            }
        }
    }
}

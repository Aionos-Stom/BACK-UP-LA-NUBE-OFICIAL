
using BackUp.Aplication.Dtos.politicaBackup;
using BackUp.Aplication.Dtos.PoliticaBackup;
using BackUp.Aplication.Interfaces.Base;
using BackUp.Aplication.Interfaces.Repositorios;
using BackUp.Domain.Base;
using BackUp.Domain.Entities.Bac;
using BackUp.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace BackUp.Persistence.Repositories
{
    public class PoliticaBackupRepository : IPoliticaBackupRepository
    {
        private readonly BackUpDbContext _context;
        private readonly ILogger<PoliticaBackupRepository> _logger;

        public PoliticaBackupRepository(BackUpDbContext context, ILogger<PoliticaBackupRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<OperationResult> ObtenerPorIdAsync(int id)
        {
            try
            {
                var politica = await _context.PoliticaBackup
                    .Include(p => p.Organizacion)
                    .Include(p => p.JobsBackup)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (politica == null)
                    return OperationResult.Failure("Política de backup no encontrada");

                var dto = new ObtenerPoliticaBackupDTO
                {
                    Id = politica.Id,
                    OrganizacionId = politica.OrganizacionId,
                    Nombre = politica.Nombre,
                    Frecuencia = politica.Frecuencia,
                    TipoBackup = politica.TipoBackup,
                    RetencionDias = politica.RetencionDias,
                    RpoMinutes = politica.RpoMinutes,
                    RtoMinutes = politica.RtoMinutes,
                    VentanaEjecucion = politica.VentanaEjecucion,
                    IsActive = politica.IsActive
                };

                return OperationResult.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener política de backup");
                return OperationResult.Failure($"Error al obtener política de backup: {ex.Message}");
            }
        }

        public async Task<OperationResult> ObtenerTodosAsync()
        {
            try
            {
                var politicas = await _context.PoliticaBackup
                    .Include(p => p.Organizacion)
                    .Include(p => p.JobsBackup)
                    .Select(p => new ObtenerPoliticaBackupDTO
                    {
                        Id = p.Id,
                        OrganizacionId = p.OrganizacionId,
                        Nombre = p.Nombre,
                        Frecuencia = p.Frecuencia,
                        TipoBackup = p.TipoBackup,
                        RetencionDias = p.RetencionDias,
                        RpoMinutes = p.RpoMinutes,
                        RtoMinutes = p.RtoMinutes,
                        VentanaEjecucion = p.VentanaEjecucion,
                        IsActive = p.IsActive
                    })
                    .ToListAsync();

                return OperationResult.Success(politicas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener políticas de backup");
                return OperationResult.Failure($"Error al obtener políticas de backup: {ex.Message}");
            }
        }

        public async Task<OperationResult> AgregarAsync(SavePoliticaBackupDTO savePoliticaBackup)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(savePoliticaBackup.Nombre))
                    return OperationResult.Failure("El nombre de la política de backup es requerido");

                var politica = new PoliticaBackup
                {
                    OrganizacionId = savePoliticaBackup.OrganizacionId,
                    Nombre = savePoliticaBackup.Nombre,
                    Frecuencia = savePoliticaBackup.Frecuencia,
                    TipoBackup = savePoliticaBackup.TipoBackup,
                    RetencionDias = savePoliticaBackup.RetencionDias,
                    RpoMinutes = savePoliticaBackup.RpoMinutes,
                    RtoMinutes = savePoliticaBackup.RtoMinutes,
                    VentanaEjecucion = savePoliticaBackup.VentanaEjecucion,
                    IsActive = true
                };

                _context.PoliticaBackup.Add(politica);
                await _context.SaveChangesAsync();

                return OperationResult.Success(politica.Id);
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
                var politica = await _context.PoliticaBackup
                    .FirstOrDefaultAsync(p => p.Id == updatePoliticaBackup.Id);

                if (politica == null)
                    return OperationResult.Failure("Política de backup no encontrada");

                politica.Nombre = updatePoliticaBackup.Nombre;
                politica.Frecuencia = updatePoliticaBackup.Frecuencia;
                politica.TipoBackup = updatePoliticaBackup.TipoBackup;
                politica.RetencionDias = updatePoliticaBackup.RetencionDias;
                politica.RpoMinutes = updatePoliticaBackup.RpoMinutes;
                politica.RtoMinutes = updatePoliticaBackup.RtoMinutes;
                politica.VentanaEjecucion = updatePoliticaBackup.VentanaEjecucion;
                politica.IsActive = updatePoliticaBackup.IsActive;

                await _context.SaveChangesAsync();

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar política de backup");
                return OperationResult.Failure($"Error al actualizar política de backup: {ex.Message}");
            }
        }

        public async Task<OperationResult> EliminarAsync(RemovePoliticaBackupDTO removePoliticaBackup)
        {
            try
            {
                var politica = await _context.PoliticaBackup
                    .FirstOrDefaultAsync(p => p.Id == removePoliticaBackup.Id);

                if (politica == null)
                    return OperationResult.Failure("Política de backup no encontrada");

                _context.PoliticaBackup.Remove(politica);
                await _context.SaveChangesAsync();

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar política de backup");
                return OperationResult.Failure($"Error al eliminar política de backup: {ex.Message}");
            }
        }

        public async Task<OperationResult> ObtenerPorOrganizacionAsync(int organizacionId)
        {
            try
            {
                var politicas = await _context.PoliticaBackup
                    .Where(p => p.OrganizacionId == organizacionId)
                    .Include(p => p.Organizacion)
                    .Include(p => p.JobsBackup)
                    .Select(p => new ObtenerPoliticaBackupDTO
                    {
                        Id = p.Id,
                        OrganizacionId = p.OrganizacionId,
                        Nombre = p.Nombre,
                        Frecuencia = p.Frecuencia,
                        TipoBackup = p.TipoBackup,
                        RetencionDias = p.RetencionDias,
                        RpoMinutes = p.RpoMinutes,
                        RtoMinutes = p.RtoMinutes,
                        VentanaEjecucion = p.VentanaEjecucion,
                        IsActive = p.IsActive
                    })
                    .ToListAsync();

                return OperationResult.Success(politicas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener políticas por organización");
                return OperationResult.Failure($"Error al obtener políticas por organización: {ex.Message}");
            }
        }

        public async Task<OperationResult> ObtenerActivasAsync()
        {
            try
            {
                var politicas = await _context.PoliticaBackup
                    .Where(p => p.IsActive)
                    .Include(p => p.Organizacion)
                    .Include(p => p.JobsBackup)
                    .Select(p => new ObtenerPoliticaBackupDTO
                    {
                        Id = p.Id,
                        OrganizacionId = p.OrganizacionId,
                        Nombre = p.Nombre,
                        Frecuencia = p.Frecuencia,
                        TipoBackup = p.TipoBackup,
                        RetencionDias = p.RetencionDias,
                        RpoMinutes = p.RpoMinutes,
                        RtoMinutes = p.RtoMinutes,
                        VentanaEjecucion = p.VentanaEjecucion,
                        IsActive = p.IsActive
                    })
                    .ToListAsync();

                return OperationResult.Success(politicas);
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
                var politicas = await _context.PoliticaBackup
                    .Where(p => p.TipoBackup == tipoBackup)
                    .Include(p => p.Organizacion)
                    .Include(p => p.JobsBackup)
                    .Select(p => new ObtenerPoliticaBackupDTO
                    {
                        Id = p.Id,
                        OrganizacionId = p.OrganizacionId,
                        Nombre = p.Nombre,
                        Frecuencia = p.Frecuencia,
                        TipoBackup = p.TipoBackup,
                        RetencionDias = p.RetencionDias,
                        RpoMinutes = p.RpoMinutes,
                        RtoMinutes = p.RtoMinutes,
                        VentanaEjecucion = p.VentanaEjecucion,
                        IsActive = p.IsActive
                    })
                    .ToListAsync();

                return OperationResult.Success(politicas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener políticas por tipo");
                return OperationResult.Failure($"Error al obtener políticas por tipo: {ex.Message}");
            }
        }
    }
}
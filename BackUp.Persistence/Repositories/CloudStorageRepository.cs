using BackUp.Aplication.Dtos.CloudStorage;
using BackUp.Aplication.Interfaces.Repositorios;
using BackUp.Domain.Base;
using BackUp.Domain.Entities.Bac;
using BackUp.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Persistence.Repositories
{
    public class CloudStorageRepository : ICloudStorageRepository
    {
        private readonly BackUpDbContext _context;

        public CloudStorageRepository(BackUpDbContext context)
        {
            _context = context;
        }

        public async Task<OperationResult> ObtenerPorIdAsync(int id)
        {
            try
            {
                var cloudStorage = await _context.CloudStorage
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cs => cs.Id == id && cs.IsActive);

                if (cloudStorage == null)
                    return OperationResult.Failure("Cloud Storage no encontrado");

                var dto = new ObtenerCloudStorageDTO
                {
                    Id = cloudStorage.Id,
                    OrganizacionId = cloudStorage.OrganizacionId,
                    Proveedor = cloudStorage.Proveedor,
                    EndpointUrl = cloudStorage.EndpointUrl,
                    TierActual = cloudStorage.TierActual,
                    CostoMensual = cloudStorage.CostoMensual,
                    IsActive = cloudStorage.IsActive,
                    FechaCreacion = DateTime.UtcNow // Ajustar según propiedad real
                };

                return OperationResult.Success(dto);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al obtener cloud storage: {ex.Message}");
            }
        }

        public async Task<OperationResult> ObtenerTodosAsync()
        {
            try
            {
                var cloudStorages = await _context.CloudStorage
                    .AsNoTracking()
                    .Where(cs => cs.IsActive)
                    .OrderBy(cs => cs.Proveedor)
                    .ToListAsync();

                var dtos = cloudStorages.Select(cs => new ObtenerCloudStorageDTO
                {
                    Id = cs.Id,
                    OrganizacionId = cs.OrganizacionId,
                    Proveedor = cs.Proveedor,
                    EndpointUrl = cs.EndpointUrl,
                    TierActual = cs.TierActual,
                    CostoMensual = cs.CostoMensual,
                    IsActive = cs.IsActive,
                    FechaCreacion = DateTime.UtcNow // Ajustar según propiedad real
                }).ToList();

                return OperationResult.Success(dtos);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al obtener cloud storages: {ex.Message}");
            }
        }

        public async Task<OperationResult> AgregarAsync(SaveCloudStorageDTO saveCloudStorage)
        {
            try
            {
                var cloudStorage = new CloudStorage
                {
                    OrganizacionId = saveCloudStorage.OrganizacionId,
                    Proveedor = saveCloudStorage.Proveedor,
                    Configuration = saveCloudStorage.Configuration,
                    EndpointUrl = saveCloudStorage.EndpointUrl,
                    TierActual = saveCloudStorage.TierActual,
                    CostoMensual = saveCloudStorage.CostoMensual,
                    IsActive = saveCloudStorage.IsActive
                };

                await _context.CloudStorage.AddAsync(cloudStorage);
                await _context.SaveChangesAsync();

                var dto = new ObtenerCloudStorageDTO
                {
                    Id = cloudStorage.Id,
                    OrganizacionId = cloudStorage.OrganizacionId,
                    Proveedor = cloudStorage.Proveedor,
                    EndpointUrl = cloudStorage.EndpointUrl,
                    TierActual = cloudStorage.TierActual,
                    CostoMensual = cloudStorage.CostoMensual,
                    IsActive = cloudStorage.IsActive,
                    FechaCreacion = DateTime.UtcNow
                };

                return OperationResult.Success(dto);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al agregar cloud storage: {ex.Message}");
            }
        }

        public async Task<OperationResult> ActualizarAsync(UpdateCloudStorageDTO updateCloudStorage)
        {
            try
            {
                var cloudStorage = await _context.CloudStorage
                    .FirstOrDefaultAsync(cs => cs.Id == updateCloudStorage.Id);

                if (cloudStorage == null)
                    return OperationResult.Failure("Cloud Storage no encontrado");

                cloudStorage.Proveedor = updateCloudStorage.Proveedor;
                cloudStorage.Configuration = updateCloudStorage.Configuration;
                cloudStorage.EndpointUrl = updateCloudStorage.EndpointUrl;
                cloudStorage.TierActual = updateCloudStorage.TierActual;
                cloudStorage.CostoMensual = updateCloudStorage.CostoMensual;
                cloudStorage.IsActive = updateCloudStorage.IsActive;

                await _context.SaveChangesAsync();

                var dto = new ObtenerCloudStorageDTO
                {
                    Id = cloudStorage.Id,
                    OrganizacionId = cloudStorage.OrganizacionId,
                    Proveedor = cloudStorage.Proveedor,
                    EndpointUrl = cloudStorage.EndpointUrl,
                    TierActual = cloudStorage.TierActual,
                    CostoMensual = cloudStorage.CostoMensual,
                    IsActive = cloudStorage.IsActive,
                    FechaCreacion = DateTime.UtcNow
                };

                return OperationResult.Success(dto);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al actualizar cloud storage: {ex.Message}");
            }
        }

        public async Task<OperationResult> EliminarAsync(RemoveCloudStorageDTO removeCloudStorage)
        {
            try
            {
                var cloudStorage = await _context.CloudStorage
                    .FirstOrDefaultAsync(cs => cs.Id == removeCloudStorage.Id);

                if (cloudStorage == null)
                    return OperationResult.Failure("Cloud Storage no encontrado");

                // Soft delete
                cloudStorage.IsActive = false;

                await _context.SaveChangesAsync();

                return OperationResult.Success("Cloud Storage eliminado correctamente");
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al eliminar cloud storage: {ex.Message}");
            }
        }

        public async Task<OperationResult> ObtenerPorOrganizacionAsync(int organizacionId)
        {
            try
            {
                var cloudStorages = await _context.CloudStorage
                    .AsNoTracking()
                    .Where(cs => cs.OrganizacionId == organizacionId && cs.IsActive)
                    .OrderBy(cs => cs.Proveedor)
                    .ToListAsync();

                var dtos = cloudStorages.Select(cs => new ObtenerCloudStorageDTO
                {
                    Id = cs.Id,
                    OrganizacionId = cs.OrganizacionId,
                    Proveedor = cs.Proveedor,
                    EndpointUrl = cs.EndpointUrl,
                    TierActual = cs.TierActual,
                    CostoMensual = cs.CostoMensual,
                    IsActive = cs.IsActive,
                    FechaCreacion = DateTime.UtcNow
                }).ToList();

                return OperationResult.Success(dtos);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al obtener cloud storages por organización: {ex.Message}");
            }
        }
    }
}

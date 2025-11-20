using BackUp.Aplication.Dtos.organizacion;
using BackUp.Aplication.Interfaces.Repositorios;
using BackUp.Domain.Base;
using BackUp.Domain.Entities.Bac;
using BackUp.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace BackUp.Persistence.Repositories
{
    public class OrganizacionRepository : IOrganizacionRepository
    {
        private readonly BackUpDbContext _context;

        public OrganizacionRepository(BackUpDbContext context)
        {
            _context = context;
        }

        public async Task<OperationResult> ObtenerPorIdAsync(int id)
        {
            try
            {
                var organizacion = await _context.Organizacion
                    .Include(o => o.CloudStorages)
                    .Include(o => o.PoliticasBackup)
                    .Include(o => o.Usuarios)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (organizacion == null)
                    return OperationResult.Failure("Organización no encontrada");

                var dto = new ObtenerOrganizacionDTO
                {
                    Id = organizacion.Id,
                    Nombre = organizacion.Nombre,
                    Configuracion = organizacion.Configuracion,
                    LicenciaValidaHasta = organizacion.LicenciaValidaHasta,
                    MaxUsuarios = organizacion.MaxUsuarios,
                    Activo = organizacion.Activo,
                    TotalCloudStorages = organizacion.CloudStorages?.Count ?? 0,
                    TotalPoliticas = organizacion.PoliticasBackup?.Count ?? 0,
                    TotalUsuarios = organizacion.Usuarios?.Count ?? 0
                };

                return OperationResult.Success(dto);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al obtener organización: {ex.Message}");
            }
        }

        public async Task<OperationResult> ObtenerTodosAsync()
        {
            try
            {
                var organizaciones = await _context.Organizacion
                    .Where(o => o.Activo)
                    .Select(o => new ObtenerOrganizacionDTO
                    {
                        Id = o.Id,
                        Nombre = o.Nombre,
                        Configuracion = o.Configuracion,
                        LicenciaValidaHasta = o.LicenciaValidaHasta,
                        MaxUsuarios = o.MaxUsuarios,
                        Activo = o.Activo,
                        TotalCloudStorages = o.CloudStorages.Count(cs => cs.IsActive),
                        TotalPoliticas = o.PoliticasBackup.Count(pb => pb.IsActive),
                        TotalUsuarios = o.Usuarios.Count(u => u.IsActive)
                    })
                    .ToListAsync();

                return OperationResult.Success(organizaciones);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al obtener organizaciones: {ex.Message}");
            }
        }

        public async Task<OperationResult> AgregarAsync(SaveOrganizacionDTO saveOrganizacion)
        {
            try
            {
                var organizacion = new Domain.Entities.Bac.Organizacion
                {
                    Nombre = saveOrganizacion.Nombre,
                    Configuracion = saveOrganizacion.Configuracion,
                    LicenciaValidaHasta = saveOrganizacion.LicenciaValidaHasta,
                    MaxUsuarios = saveOrganizacion.MaxUsuarios,
                    Activo = saveOrganizacion.Activo,
                    
                };

                _context.Organizacion.Add(organizacion);
                await _context.SaveChangesAsync();

                return OperationResult.Success(organizacion.Id);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al crear organización: {ex.Message}");
            }
        }

        public async Task<OperationResult> ActualizarAsync(UpdateOrganizacionDTO updateOrganizacion)
        {
            try
            {
                var organizacion = await _context.Organizacion
                    .FirstOrDefaultAsync(o => o.Id == updateOrganizacion.Id);

                if (organizacion == null)
                    return OperationResult.Failure("Organización no encontrada");

                organizacion.Nombre = updateOrganizacion.Nombre;
                organizacion.Configuracion = updateOrganizacion.Configuracion;
                organizacion.LicenciaValidaHasta = updateOrganizacion.LicenciaValidaHasta;
                organizacion.MaxUsuarios = updateOrganizacion.MaxUsuarios;
                organizacion.Activo = updateOrganizacion.Activo;

                await _context.SaveChangesAsync();

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al actualizar organización: {ex.Message}");
            }
        }

        public async Task<OperationResult> EliminarAsync(RemoveOrganizacionDTO removeOrganizacion)
        {
            try
            {
                var organizacion = await _context.Organizacion
                    .FirstOrDefaultAsync(o => o.Id == removeOrganizacion.Id);

                if (organizacion == null)
                    return OperationResult.Failure("Organización no encontrada");

                // Soft delete
                organizacion.Activo = false;
                await _context.SaveChangesAsync();

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al eliminar organización: {ex.Message}");
            }
        }
    }
}

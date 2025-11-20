using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BackUp.Domain.Base;


namespace BackUp.Aplication.Interfaces.Base
{
    public abstract class BaseService<TEntity, TSaveDto, TUpdateDto, TRemoveDto> : IBaseService<TSaveDto, TUpdateDto, TRemoveDto>
       where TEntity : class
    {
        protected readonly DbContext _context;
        protected readonly ILogger _logger;
        protected readonly DbSet<TEntity> _dbSet;

        public BaseService(DbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
            _dbSet = context.Set<TEntity>();
        }

        public virtual async Task<OperationResult> AgregarAsync(TSaveDto dto)
        {
            if (dto is null)
                return OperationResult.Failure("El DTO no puede ser nulo.");

            try
            {
                var entity = MapSaveDtoToEntity(dto);
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Entidad agregada correctamente");
                return OperationResult.Success("Entidad agregada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar entidad.");
                return OperationResult.Failure("Error al agregar entidad.");
            }
        }

        public virtual async Task<OperationResult> EliminarAsync(TRemoveDto dto)
        {
            try
            {
                var entity = MapRemoveDtoToEntity(dto);
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Entidad eliminada correctamente.");
                return OperationResult.Success("Entidad eliminada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar entidad.");
                return OperationResult.Failure("Error al eliminar entidad.");
            }
        }

        public virtual async Task<OperationResult> ObtenerTodosAsync()
        {
            try
            {
                var data = await _dbSet.ToListAsync();
                return OperationResult.Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener entidades.");
                return OperationResult.Failure("Error al obtener entidades.");
            }
        }

        public virtual async Task<OperationResult> ObtenerPorIdAsync(int id)
        {
            if (id <= 0)
                return OperationResult.Failure("El ID debe ser mayor que cero.");

            try
            {
                var entity = await _dbSet.FindAsync(id);
                return entity == null
                    ? OperationResult.Failure("Entidad no encontrada.")
                    : OperationResult.Success(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener entidad por ID.");
                return OperationResult.Failure("Error al obtener entidad.");
            }
        }

        public virtual async Task<OperationResult> ActualizarAsync(TUpdateDto dto)
        {
            try
            {
                var entity = MapUpdateDtoToEntity(dto);
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
                return OperationResult.Success("Entidad actualizada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar entidad.");
                return OperationResult.Failure("Error al actualizar entidad.");
            }
        }

        // Métodos abstractos para que el hijo haga el mapeo entre DTOs y entidad
        protected abstract TEntity MapSaveDtoToEntity(TSaveDto dto);
        protected abstract TEntity MapUpdateDtoToEntity(TUpdateDto dto);
        protected abstract TEntity MapRemoveDtoToEntity(TRemoveDto dto);
    }
}

using Microsoft.EntityFrameworkCore;
using BackUp.Domain.Base;
using BackUp.Persistence.Context;
using BackUp.Aplication.Interfaces.Base;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Persistence.Base
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly BackUpDbContext _context;
        protected readonly DbSet<T> _dbSet;

        protected BaseRepository(BackUpDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<OperationResult> AddAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                return OperationResult.Success("Registro creado exitosamente");
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al crear {typeof(T).Name}: {ex.Message}");
            }
        }

        public virtual async Task<OperationResult> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                return entity == null
                    ? OperationResult.Failure("Registro no encontrado")
                    : OperationResult.Success(entity);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al obtener {typeof(T).Name}: {ex.Message}");
            }
        }

        public virtual async Task<OperationResult> GetAllAsync(Expression<Func<T, bool>>? filter = null)
        {
            try
            {
                var query = _dbSet.AsQueryable();

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                var result = await query.ToListAsync();
                return OperationResult.Success(result);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al obtener lista de {typeof(T).Name}: {ex.Message}");
            }
        }

        public virtual async Task<OperationResult> UpdateAsync(T entity)
        {
            try
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
                return OperationResult.Success("Registro actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al actualizar {typeof(T).Name}: {ex.Message}");
            }
        }

        public virtual async Task<OperationResult> DeleteAsync(int id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                {
                    return OperationResult.Failure("Registro no encontrado");
                }

                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
                return OperationResult.Success("Registro eliminado exitosamente");
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al eliminar {typeof(T).Name}: {ex.Message}");
            }
        }

        public async Task<OperationResult> SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return OperationResult.Success("Cambios guardados exitosamente");
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al guardar cambios: {ex.Message}");
            }
        }
    }
}


using BackUp.Domain.Base;
using System.Linq.Expressions;
namespace BackUp.Aplication.Interfaces.Base
{
    public interface IRepository<T> where T : class
    {
        public interface IRepository<T> where T : class
        {
            Task<OperationResult> AddAsync(T entity);
            Task<OperationResult> GetByIdAsync(int id);
            Task<OperationResult> GetAllAsync(Expression<Func<T, bool>>? filter = null);
            Task<OperationResult> UpdateAsync(T entity);
            Task<OperationResult> DeleteAsync(int id);
            Task<OperationResult> SaveChangesAsync();
        }
    }
}

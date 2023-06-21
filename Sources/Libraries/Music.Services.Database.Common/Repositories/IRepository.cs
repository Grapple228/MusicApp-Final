using System.Linq.Expressions;
using Music.Shared.Common;

namespace Music.Services.Database.Common.Repositories;

public interface IRepository<T> where T : IModel
{
    Task<IReadOnlyCollection<T>> GetAllAsync();
    Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter);
    Task<IReadOnlyCollection<T>> GetAllAsync(IEnumerable<Guid> ids);
    Task<PageResult<T>> GetAllFromPageAsync(long pageNumber, long countPerPage);
    Task<PageResult<T>> GetAllFromPageAsync(long pageNumber, long countPerPage, Expression<Func<T, bool>> filter);
    Task<T?> GetAsync(Guid id);
    Task<T?> GetAsync(Expression<Func<T, bool>> filter);
    Task CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task RemoveAsync(Guid id);
    Task<long> GetItemsCountAsync();
    Task<long> GetItemsCountAsync(Expression<Func<T, bool>> filter);
}
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenIddict.UI.Suite.Core;

public interface IAsyncRepository<TEntity, TKey>
  where TEntity : class
{
    Task<TEntity> GetByIdAsync(TKey id);
    Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> spec);
    Task<TEntity> AddAsync(TEntity entity);
    Task<List<TEntity>> AddRangeAsync(List<TEntity> entities);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
    Task DeleteRangeAsync(List<TEntity> entities);
    Task<int> CountAsync(ISpecification<TEntity> spec);
}

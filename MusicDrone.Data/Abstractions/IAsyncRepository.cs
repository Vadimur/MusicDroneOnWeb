using System.Collections.Generic;
using System.Threading.Tasks;
using MusicDrone.Data.Models;

namespace MusicDrone.Data.Abstractions
{
    public interface IAsyncRepository<TKey, TEntity> : IRepository where TEntity : BaseEntity<TKey>
    {
        Task<TEntity> GetByIdAsync(TKey id);
        Task<IReadOnlyList<TEntity>> ListAllAsync();
        Task<TEntity> AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        void Delete(TEntity entity);
        Task<int> Count();
    }
}
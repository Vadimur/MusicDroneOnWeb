using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicDrone.Data.Abstractions;
using MusicDrone.Data.Models;
using MusicDrone.Data.Services.Abstraction;

namespace MusicDrone.Data.Repositories
{
    public abstract class GenericRepository<TKey, TEntity> : IAsyncRepository<TKey, TEntity> where TEntity : BaseEntity<TKey>
    {
        protected MusicDroneDbContext DbContext { get; }

        protected GenericRepository(MusicDroneDbContext musicDroneDbContext)
        {
            DbContext = musicDroneDbContext;
        }
        
        public virtual async Task<TEntity> GetByIdAsync(TKey id)
        {
            return await DbContext.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task<IReadOnlyList<TEntity>> ListAllAsync()
        {
            return await DbContext.Set<TEntity>().ToListAsync();
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            var result = await DbContext.Set<TEntity>().AddAsync(entity);
            return result.Entity;
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            TEntity find = await this.GetByIdAsync(entity.Id);
            DbContext.Entry(find).CurrentValues.SetValues(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            DbContext.Set<TEntity>().Remove(entity);
        }
        
        public virtual async Task<int> Count()
        {
            return await DbContext.Set<TEntity>().CountAsync();
        }
    }
}
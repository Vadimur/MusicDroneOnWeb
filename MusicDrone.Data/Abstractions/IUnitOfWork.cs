using System;
using System.Threading.Tasks;

namespace MusicDrone.Data.Abstractions
{
    
    public interface IUnitOfWork : IDisposable
    {
        TRepository GetRepository<TRepository>() where TRepository : IRepository;
        Task<bool> SaveChanges();
    }
}
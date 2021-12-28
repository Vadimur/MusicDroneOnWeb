using System;
using System.Collections;
using System.Threading.Tasks;
using MusicDrone.Data.Abstractions;
using MusicDrone.Data.Models;
using MusicDrone.Data.Repositories;
using MusicDrone.Data.Services.Abstraction;

namespace MusicDrone.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MusicDroneDbContext _context;
        private Hashtable _repositories = new();

        public UnitOfWork(MusicDroneDbContext context, IRoomRepository roomRepo, 
            IRoomsUsersRepository roomsUsersRepo, IUserRepository userRepo,
            IMusicRepository musicRepo, IFileDescRepository fileDescRepo)
        {
            _context = context;
            _repositories.Add(typeof(IRoomRepository), roomRepo);
            _repositories.Add(typeof(IRoomsUsersRepository), roomsUsersRepo);
            _repositories.Add(typeof(IUserRepository), userRepo);
            _repositories.Add(typeof(IMusicRepository), userRepo);
            _repositories.Add(typeof(IFileDescRepository), fileDescRepo);
        }

        public TRepository GetRepository<TRepository>() where TRepository : IRepository
        {
            var type = typeof(TRepository);

            if (!_repositories.ContainsKey(type))
                throw new ArgumentException("Repository does not exist!");

            return (TRepository)_repositories[type];
        }
        
        public async Task<bool> SaveChanges()
        {
            int result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
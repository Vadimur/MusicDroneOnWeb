using MusicDrone.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicDrone.IntegrationTests.Shared
{
    public static class Extensions
    {
        public static async Task SaveEntity<T>(this MusicDroneDbContext context, T entity) where T : class
        {
            await context.Set<T>().AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public static async Task SaveEntityRange<T>(this MusicDroneDbContext context, IEnumerable<T> entityRange) where T : class
        {
            await context.Set<T>().AddRangeAsync(entityRange);
            await context.SaveChangesAsync();
        }
    }
}

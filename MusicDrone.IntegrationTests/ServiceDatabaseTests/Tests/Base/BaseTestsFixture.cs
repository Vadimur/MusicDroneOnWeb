using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MusicDrone.Data;
using MusicDrone.IntegrationTests.Shared;
using System;
using System.Data.Common;

namespace MusicDrone.IntegrationTests.ServiceDatabaseTests.Tests.Base
{
    public class BaseTestsFixture : IDisposable
    {
        public DbConnection Connection { get; }
        
        private static readonly object _lock = new ();
        private static bool _databaseInitialized;

        public BaseTestsFixture()
        {
            Connection = new SqlConnection(@"Data Source=localhost;Integrated Security=true;Initial Catalog=TestMusicDroneIdentityDb");

            Seed();

            Connection.Open();
        }

        public MusicDroneDbContext CreateContext(DbTransaction transaction = null)
        {
            var context = new MusicDroneDbContext(new DbContextOptionsBuilder<MusicDroneDbContext>().UseSqlServer(Connection).Options);

            if (transaction != null)
            {
                context.Database.UseTransaction(transaction);
            }

            return context;
        }

        private void Seed()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();

                        context.Users.Add(SharedTestData.DefaultUser);

                        context.SaveChanges();
                    }

                    _databaseInitialized = true;
                }
            }
        }

        public void Dispose()
        {
            Connection.Dispose();
        }
    }
}

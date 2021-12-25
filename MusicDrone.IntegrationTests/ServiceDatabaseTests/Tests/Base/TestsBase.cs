using MusicDrone.Data;
using System;
using System.Data.Common;
using Xunit;

namespace MusicDrone.IntegrationTests.ServiceDatabaseTests.Tests.Base
{
    public class TestsBase : IClassFixture<BaseTestsFixture>, IDisposable
    {
        private DbTransaction _transaction;
        protected MusicDroneDbContext Context { get; }
        protected BaseTestsFixture Fixture { get; }

        public TestsBase(BaseTestsFixture fixture)
        {
            Fixture = fixture;
            _transaction = Fixture.Connection.BeginTransaction();
            Context = Fixture.CreateContext(_transaction);
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            Context?.Dispose();
        }
    }
}

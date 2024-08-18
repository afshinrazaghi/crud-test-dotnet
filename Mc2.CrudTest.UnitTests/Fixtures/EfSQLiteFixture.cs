using Mc2.CrudTest.Presentation.Infrastructure.Command.Context;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.UnitTests.Fixtures
{
    public class EfSQLiteFixture : IAsyncLifetime, IDisposable
    {
        private const string ConnectionString = "Data Source=:memory:";
        private readonly SqliteConnection _connection;
        public EfSQLiteFixture()
        {
            _connection = new SqliteConnection(ConnectionString);
            _connection.Open();

            DbContextOptionsBuilder<WriteDbContext> buidler = new DbContextOptionsBuilder<WriteDbContext>()
                .UseSqlite(_connection);
            Context = new WriteDbContext(buidler.Options);
        }


        public WriteDbContext Context { get; }

        #region IAsyncLifeTime
        public async Task InitializeAsync()
        {
            await Context.Database.EnsureDeletedAsync();
            await Context.Database.EnsureCreatedAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;
        #endregion

        #region IDisposable
        private bool _disposed;

        ~EfSQLiteFixture() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _connection?.Dispose();
                Context?.Dispose();
            }

            _disposed = true;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Testcontainers.MsSql;
using Mc2.CrudTest.Presentation.Infrastructure.Command.Context;
using Microsoft.EntityFrameworkCore;

namespace Mc2.CrudTest.AcceptanceTests.Fixtures
{
    public class EFSQLContextFixture : IDisposable
    {
        public EFSQLContextFixture(string connectionString)
        {
            DbContextOptionsBuilder<WriteDbContext> builder = new DbContextOptionsBuilder<WriteDbContext>()
                .UseSqlServer(connectionString);
            Context = new WriteDbContext(builder.Options);
        }

        public WriteDbContext Context { get; }


        #region IDisposable
        private bool disposed;

        ~EFSQLContextFixture() => Dispose(false);

        public void Dispose()
        {

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                Context.Dispose();
            }

            disposed = true;
        }



        #endregion
    }
}

using Mc2.CrudTest.Presentation.Infrastructure.Command.Context;
using Mc2.CrudTest.Presentation.Infrastructure.Query.Context;
using Mc2.CrudTest.Presentation.Shared.AppSettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mongo2Go;
using MongoDB.Driver;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.AcceptanceTests.Fixtures
{
    public class MongoContextFixture : IDisposable
    {
        public const string ConnectionString = "";

        public MongoContextFixture(string connectionString)
        {
            IOptions<ConnectionOptions> options = Options.Create<ConnectionOptions>(new ConnectionOptions { NoSqlConnection = connectionString });
            Context = new NoSqlDbContext(options, Substitute.For<ILogger<NoSqlDbContext>>());
        }

        public NoSqlDbContext Context { get; }

        #region IDisposable
        private bool disposed;

        ~MongoContextFixture() => Dispose(false);

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
            }

            disposed = true;
        }



        #endregion
    }
}

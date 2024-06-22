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
    public class MongoFixture
    {
        public const string ConnectionString = "";
        private readonly MongoDbRunner _runner;
        private readonly IMongoDatabase _database;


        public MongoFixture()
        {
            _runner = MongoDbRunner.Start();
            IOptions<ConnectionOptions> options = Options.Create<ConnectionOptions>(new ConnectionOptions { NoSqlConnection = _runner.ConnectionString });
            Context = new NoSqlDbContext(options, Substitute.For<ILogger<NoSqlDbContext>>());
        }

        public NoSqlDbContext Context { get; }
    }
}

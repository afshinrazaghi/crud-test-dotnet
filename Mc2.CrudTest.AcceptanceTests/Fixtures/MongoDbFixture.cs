using Mc2.CrudTest.AcceptanceTests.Helper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MongoDb;
using Xunit;

namespace Mc2.CrudTest.AcceptanceTests.Fixtures
{
    public class MongoDbFixture : IAsyncLifetime
    {
        public MongoDbContainer Container { get; }
        public MongoDbFixture()
        {
            IConfiguration configuration = ConfigurationHelper.GetConfiguration();
            int port = configuration.GetSection("MongoDb").GetValue<int>("PORT");
            Container = new MongoDbBuilder()
                .WithImage("mongo:latest")
                .WithPortBinding(port, true)
                .Build();
        }

        public Task DisposeAsync()
            => Container.DisposeAsync().AsTask();

        public Task InitializeAsync()
            => Container.StartAsync();

    }
}

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
            Container = new MongoDbBuilder()
                .WithImage("mongo:latest")
                .WithPortBinding(27017, true)
                .Build();

            Task.Run(InitializeAsync).Wait();
        }

        public Task DisposeAsync()
            => Container.DisposeAsync().AsTask();

        public Task InitializeAsync()
            => Container.StartAsync();

    }
}

using DotNet.Testcontainers.Builders;
using NSubstitute.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using Xunit;


namespace Mc2.CrudTest.AcceptanceTests.Fixtures
{
    public class MsSqlFixture : IAsyncLifetime
    {
        public MsSqlContainer Container { get; }

        public MsSqlFixture()
        {
            Container = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server")
                .WithEnvironment("ACCEPT_EULA", "Y")
                .WithEnvironment("SA_PASSWORD", "yourStrong(!)Password")
                .WithPortBinding(1433, true)
                .Build();

            Task.Run(InitializeAsync).Wait();

        }

        public Task DisposeAsync()
            => Container.DisposeAsync().AsTask();

        public Task InitializeAsync()
            => Container.StartAsync();
    }
}

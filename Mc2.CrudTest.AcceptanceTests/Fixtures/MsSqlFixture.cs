using DotNet.Testcontainers.Builders;
using Mc2.CrudTest.AcceptanceTests.Helper;
using Microsoft.Extensions.Configuration;
using NSubstitute.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
            IConfiguration configuration = ConfigurationHelper.GetConfiguration();
            string password = configuration.GetSection("MSSqlServer").GetValue<string>("SA_PASSWORD")!;
            int port = configuration.GetSection("MSSqlServer").GetValue<int>("PORT")!;
            Container = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server")
                .WithEnvironment("ACCEPT_EULA", "Y")
                .WithEnvironment("SA_PASSWORD", password)
                .WithPortBinding(port, true)
                .Build();
        }

        public Task DisposeAsync()
            => Container.DisposeAsync().AsTask();

        public Task InitializeAsync()
            => Container.StartAsync();
    }
}

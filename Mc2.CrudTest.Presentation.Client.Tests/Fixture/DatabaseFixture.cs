using Mc2.CrudTest.Presentation.Application.Common.Interfaces.Persistence.Query;
using Mc2.CrudTest.Presentation.Infrastructure.Command.Context;
using Mc2.CrudTest.Presentation.Infrastructure.Query.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Client.Tests.Fixture
{
    public class DatabaseFixture : IDisposable
    {
        private readonly MongoDbFixture _mongoDbFixture;
        private readonly MsSqlFixture _msSqlFixture;
        public readonly string BaseUrl = "https://localhost:7045";
        WebApplicationFactory<Mc2.CrudTest.Presentation.Program> _webApplicationFactory { get; }
        WebApplicationFactory<Mc2.CrudTest.Presentation.Client.Program> _clientApplicationFactory { get; }
        public DatabaseFixture(MongoDbFixture mongoDbFixture, MsSqlFixture msSqlFixture)
        {
            _mongoDbFixture = mongoDbFixture;
            _msSqlFixture = msSqlFixture;
            _webApplicationFactory = InitializeWebAppFactory();
            _clientApplicationFactory = InitializeClientAppFactory();
        }

        private WebApplicationFactory<Mc2.CrudTest.Presentation.Program> InitializeWebAppFactory(
           Action<IServiceCollection>? configureServices = null,
           Action<IServiceScope>? configureServiceScope = null)
        {
            return new WebApplicationFactory<Mc2.CrudTest.Presentation.Program>()
                .WithWebHostBuilder(hostBuilder =>
                {
                    hostBuilder.UseSetting("ConnectionStrings:SqlConnection", _msSqlFixture.Container.GetConnectionString());
                    hostBuilder.UseSetting("ConnectionStrings:NoSqlConnection", _mongoDbFixture.Container.GetConnectionString());

                    hostBuilder.UseEnvironment(Environments.Development);

                    hostBuilder.ConfigureServices(services =>
                    {
                        services.RemoveAll<WriteDbContext>();
                        services.RemoveAll<DbContextOptions<WriteDbContext>>();
                        services.RemoveAll<EventStoreDbContext>();
                        services.RemoveAll<DbContextOptions<EventStoreDbContext>>();
                        services.RemoveAll<ISynchronizeDb>();

                        services.AddDbContext<WriteDbContext>(
                            options => options.UseSqlServer(new SqlConnection(_msSqlFixture.Container.GetConnectionString()))
                        );

                        services.AddDbContext<EventStoreDbContext>(options =>
                        {
                            options.UseSqlServer(new SqlConnection(_msSqlFixture.Container.GetConnectionString()));
                        });

                        services.AddSingleton<IReadDbContext, NoSqlDbContext>();
                        services.AddSingleton<ISynchronizeDb, NoSqlDbContext>();

                        configureServices?.Invoke(services);

                        using ServiceProvider serviceProvider = services.BuildServiceProvider(true);
                        using IServiceScope serviceScope = serviceProvider.CreateScope();

                        WriteDbContext writeDbContext = serviceScope.ServiceProvider.GetRequiredService<WriteDbContext>();
                        writeDbContext.Database.EnsureCreated();

                        //services.AddSingleton(_ => Substitute.For<EventStoreDbContext>());
                        EventStoreDbContext eventStoreDbContext = serviceScope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
                        eventStoreDbContext.Database.EnsureCreated();

                        configureServiceScope?.Invoke(serviceScope);

                        writeDbContext.Dispose();
                        eventStoreDbContext.Dispose();
                    });
                });
        }

        private WebApplicationFactory<Mc2.CrudTest.Presentation.Client.Program> InitializeClientAppFactory(
            Action<IServiceCollection>? configureServices = null,
            Action<IServiceScope>? configureServiceScope = null)
        {
            return new WebApplicationFactory<Mc2.CrudTest.Presentation.Client.Program>()
                .WithWebHostBuilder(hostBuilder =>
                {
                    hostBuilder.ConfigureServices(services =>
                    {
                        configureServices?.Invoke(services);

                        using ServiceProvider serviceProvider = services.BuildServiceProvider(true);
                        using IServiceScope serviceScope = serviceProvider.CreateScope();

                        configureServiceScope?.Invoke(serviceScope);

                    });

                });
        }

        private static WebApplicationFactoryClientOptions CreateClientOptions() => new WebApplicationFactoryClientOptions { AllowAutoRedirect = false };

        #region Dispose
        private bool disposed;
        ~DatabaseFixture() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            _mongoDbFixture.DisposeAsync().GetAwaiter().GetResult();
            _msSqlFixture.DisposeAsync().GetAwaiter().GetResult();
            _webApplicationFactory.Dispose();

            disposed = true;
        }

        #endregion
    }
}

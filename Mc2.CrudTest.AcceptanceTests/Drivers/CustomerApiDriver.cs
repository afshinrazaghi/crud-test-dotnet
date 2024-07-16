using Mc2.CrudTest.AcceptanceTests.Fixtures;
using Mc2.CrudTest.Presentation;
using Mc2.CrudTest.Presentation.Application.Common.Interfaces.Persistence.Query;
using Mc2.CrudTest.Presentation.Application.Features.Customers.Commands;
using Mc2.CrudTest.Presentation.Infrastructure.Command.Context;
using Mc2.CrudTest.Presentation.Infrastructure.Query.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Net.Mime;
using Mc2.CrudTest.Presentation.Shared.Extensions;
using Mc2.CrudTest.Presentation.Application.Features.Customers.Queries;
using Mc2.CrudTest.Presentation.Application.Models;
using Microsoft.Data.SqlClient;

namespace Mc2.CrudTest.AcceptanceTests.Drivers
{
    public class CustomerApiDriver : IDisposable
    {
        private readonly MongoDbFixture _mongoDbFixture;
        private readonly MsSqlFixture _msSqlFixture;
        private const string EndPoint = "/api/Customer";
        WebApplicationFactory<Program> _webApplicationFactory { get; }
        public CustomerApiDriver(MongoDbFixture mongoDbFixture, MsSqlFixture msSqlFixture)
        {
            _mongoDbFixture = mongoDbFixture;
            _msSqlFixture = msSqlFixture;
            _webApplicationFactory = InitializeWebAppFactory();
        }

        public async Task<HttpResponseMessage> CreateCustomerAsync(CreateCustomerCommand command)
        {
            var httpClient = _webApplicationFactory.CreateClient(CreateClientOptions());
            var commandAsJsonString = command.ToJson();
            using var jsonContent = new StringContent(commandAsJsonString, System.Text.Encoding.UTF8, MediaTypeNames.Application.Json);
            var res = await httpClient.PostAsync(EndPoint, jsonContent);
            return res;
        }

        public async Task<HttpResponseMessage> GetAllCustomersAsync(GetAllCustomerQuery query)
        {
            var httpClient = _webApplicationFactory.CreateClient(CreateClientOptions());
            var res = await httpClient.GetAsync(EndPoint);
            return res;
        }

        public async Task<HttpResponseMessage> UpdateCustomerAsync(UpdateCustomerCommand command)
        {
            var httpClient = _webApplicationFactory.CreateClient(CreateClientOptions());
            var commandAsJsonString = command.ToJson();
            using var jsonContent = new StringContent(commandAsJsonString, System.Text.Encoding.UTF8, MediaTypeNames.Application.Json);
            var res = await httpClient.PutAsync(EndPoint, jsonContent);
            return res;
        }

        public async Task<HttpResponseMessage> DeleteCustomerAsync(DeleteCustomerCommand command)
        {
            var httpClient = _webApplicationFactory.CreateClient(CreateClientOptions());
            var commandAsJsonString = command.ToJson();
            using var jsonContent = new StringContent(commandAsJsonString, System.Text.Encoding.UTF8, MediaTypeNames.Application.Json);
            using HttpRequestMessage httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(EndPoint, UriKind.Relative),
                Content = jsonContent
            };
            var res = await httpClient.SendAsync(httpRequest);
            return res;
        }

        private WebApplicationFactory<Program> InitializeWebAppFactory(
            Action<IServiceCollection>? configureServices = null,
            Action<IServiceScope>? configureServiceScope = null)
        {
            return new WebApplicationFactory<Program>()
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

                        using var serviceProvider = services.BuildServiceProvider(true);
                        using var serviceScope = serviceProvider.CreateScope();

                        var writeDbContext = serviceScope.ServiceProvider.GetRequiredService<WriteDbContext>();
                        writeDbContext.Database.EnsureCreated();

                        //services.AddSingleton(_ => Substitute.For<EventStoreDbContext>());
                        var eventStoreDbContext = serviceScope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
                        eventStoreDbContext.Database.EnsureCreated();

                        configureServiceScope?.Invoke(serviceScope);

                        writeDbContext.Dispose();
                        eventStoreDbContext.Dispose();
                    });
                });
        }

        private static WebApplicationFactoryClientOptions CreateClientOptions() => new WebApplicationFactoryClientOptions { AllowAutoRedirect = false };


        public string GetSqlConnectionString()
            => _msSqlFixture.Container.GetConnectionString();

        public string GetMongoConnectionString()
            => _mongoDbFixture.Container.GetConnectionString();

        #region Dispose
        private bool disposed;
        ~CustomerApiDriver() => Dispose(false);

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
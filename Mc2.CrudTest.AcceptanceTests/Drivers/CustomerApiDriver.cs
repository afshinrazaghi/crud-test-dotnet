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
using Mc2.CrudTest.Presentation.Application.Features.Customers.Responses;
using Mc2.CrudTest.Presentation.Shared.Models;
using Mc2.CrudTest.AcceptanceTests.Support;

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

        public async Task<ApiResponse<CreatedCustomerResponse>> CreateCustomerAsync(CreateCustomerCommand command, CancellationToken cancellationToken)
        {
            HttpClient httpClient = _webApplicationFactory.CreateClient(CreateClientOptions());
            string commandAsJsonString = command.ToJson();
            using HttpContent jsonContent = new StringContent(commandAsJsonString, System.Text.Encoding.UTF8, MediaTypeNames.Application.Json);
            HttpResponseMessage res = await httpClient.PostAsync(EndPoint, jsonContent, cancellationToken);
            ApiResponse<CreatedCustomerResponse> result = (await res.Content.ReadAsStringAsync()).FromJson<ApiResponse<CreatedCustomerResponse>>();
            return result;
        }

        public async Task<ApiResponse<IEnumerable<CustomerQueryModel>>> GetAllCustomersAsync(CancellationToken cancellationToken)
        {
            HttpClient httpClient = _webApplicationFactory.CreateClient(CreateClientOptions());
            using HttpResponseMessage res = await httpClient.GetAsync(EndPoint, cancellationToken);
            ApiResponse<IEnumerable<CustomerQueryModel>> result = (await res.Content.ReadAsStringAsync()).FromJson<ApiResponse<IEnumerable<CustomerQueryModel>>>();
            return result;
        }

        public async Task<ApiResponse> UpdateCustomerAsync(UpdateCustomerCommand command, CancellationToken cancellationToken)
        {
            HttpClient httpClient = _webApplicationFactory.CreateClient(CreateClientOptions());
            string commandAsJsonString = command.ToJson();
            HttpContent jsonContent = new StringContent(commandAsJsonString, System.Text.Encoding.UTF8, MediaTypeNames.Application.Json);
            HttpResponseMessage res = await httpClient.PutAsync(EndPoint, jsonContent, cancellationToken);
            ApiResponse response = (await res.Content.ReadAsStringAsync()).FromJson<ApiResponse>();
            return response;
        }

        public async Task<ApiResponse> DeleteCustomerAsync(DeleteCustomerCommand command, CancellationToken cancellationToken)
        {
            HttpClient httpClient = _webApplicationFactory.CreateClient(CreateClientOptions());
            string commandAsJsonString = command.ToJson();
            using HttpContent jsonContent = new StringContent(commandAsJsonString, System.Text.Encoding.UTF8, MediaTypeNames.Application.Json);
            using HttpRequestMessage httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(EndPoint, UriKind.Relative),
                Content = jsonContent
            };
            HttpResponseMessage res = await httpClient.SendAsync(httpRequest, cancellationToken);
            ApiResponse result = (await res.Content.ReadAsStringAsync()).FromJson<ApiResponse>();
            return result;
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
                        services.RemoveDbContexts();

                        services.AddDbContexts(_msSqlFixture.Container.GetConnectionString());

                        configureServices?.Invoke(services);

                        using ServiceProvider serviceProvider = services.BuildServiceProvider(true);
                        using IServiceScope serviceScope = serviceProvider.CreateScope();

                        WriteDbContext writeDbContext = serviceScope.ServiceProvider.GetRequiredService<WriteDbContext>();
                        writeDbContext.Database.EnsureCreated();

                        EventStoreDbContext eventStoreDbContext = serviceScope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
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

            if (disposing)
            {
                _mongoDbFixture.DisposeAsync().GetAwaiter().GetResult();
                _msSqlFixture.DisposeAsync().GetAwaiter().GetResult();
                _webApplicationFactory.Dispose();
            }

            disposed = true;
        }

        #endregion
    }
}
using FluentAssertions;
using Mc2.CrudTest.AcceptanceTests.Fixtures;
using Mc2.CrudTest.Presentation;
using Mc2.CrudTest.Presentation.Application.Features.Customers.CommandHandlers;
using Mc2.CrudTest.Presentation.Application.Features.Customers.Commands;
using Mc2.CrudTest.Presentation.Infrastructure.Command;
using Mc2.CrudTest.Presentation.Infrastructure.Command.Persistence;
using Mc2.CrudTest.Presentation.Shared.SharedKernel.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using TechTalk.SpecFlow;
using Xunit;
using Testcontainers;
using Testcontainers.MsSql;
using Testcontainers.MongoDb;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mc2.CrudTest.Presentation.Infrastructure.Command.Context;
using Microsoft.EntityFrameworkCore;
using Mc2.CrudTest.Presentation.Application.Common.Interfaces.Persistence.Query;
using Mc2.CrudTest.Presentation.Infrastructure.Query.Context;
using MongoDB.Bson;
using System.Net.Mime;
using Mc2.CrudTest.Presentation.Application.Features.Customers.Queries;
using Mc2.CrudTest.Presentation.Shared.Extensions;
using Mc2.CrudTest.Presentation.Application.Models;
using Docker.DotNet.Models;
using Polly;
using System.Net.Http.Headers;
using Mc2.CrudTest.Presentation.Domain.ValueObjects;
using Mc2.CrudTest.AcceptanceTests.Drivers;
using Mc2.CrudTest.Presentation.Shared.Models;

namespace Mc2.CrudTest.AcceptanceTests
{
    [Binding]
    public class UserCanApplyCRUDOperationsAgainstTheCustomersStepDefinitions : ICollectionFixture<MongoDbFixture>, IClassFixture<MsSqlFixture>
    {

        private readonly CustomerApiDriver _customerApiDriver;
        public UserCanApplyCRUDOperationsAgainstTheCustomersStepDefinitions(CustomerApiDriver customerApiDriver)
        {
            _customerApiDriver = customerApiDriver;
        }

        [Given(@"PlatForm has ""([^""]*)"" record of customers")]
        public void GivenPlatFormHasRecordOfCustomers(string p0)
        {
            p0.Should().Be("0");
        }

        [When(@"user send command to create new customer with following information")]
        public async Task WhenUserSendCommandToCreateNewCustomerWithFollowingInformation(Table table)
        {
            var details = table.Rows.First();
            var command = new CreateCustomerCommand
            {
                FirstName = details["FirstName"],
                LastName = details["LastName"],
                PhoneNumber = details["PhoneNumber"],
                Email = details["Email"],
                BankAccountNumber = details["BankAccountNumber"],
                DateOfBirth = Convert.ToDateTime(details["DateOfBirth"])
            };

            using var res = await _customerApiDriver.CreateCustomerAsync(command);
            var str = await res.Content.ReadAsStringAsync();
        }

        [Then(@"user send query and receive ""([^""]*)"" record of customer with following information")]
        public async Task ThenUserSendQueryAndReceiveRecordOfCustomerWithFollowingInformation(string p0, Table table)
        {
            var query = new GetAllCustomerQuery();
            using var res = await _customerApiDriver.GetAllCustomersAsync(query);
            var response = (await res.Content.ReadAsStringAsync()).FromJson<ApiResponse<IEnumerable<CustomerQueryModel>>>();
            var details = table.Rows.First();
            response.Result.Where(x =>
                    x.FirstName == details["FirstName"] &&
                    x.LastName == details["LastName"] &&
                    x.PhoneNumber == details["PhoneNumber"] &&
                    x.Email == details["Email"] &&
                    x.DateOfBirth == Convert.ToDateTime(details["DateOfBirth"]) &&
                    x.BankAccountNumber == details["BankAccountNumber"]).Count().ToString().Should().Be(p0);
        }

        [When(@"user send command to update existing customer with email of ""([^""]*)"" with following information")]
        public async Task WhenUserSendCommandToUpdateExistingCustomerWithEmailOfWithFollowingInformation(string p0, Table table)
        {
            var details = table.Rows.First();
            var command = new UpdateCustomerCommand()
            {
                OriginalEmail = p0,
                FirstName = details["FirstName"],
                LastName = details["LastName"],
                PhoneNumber = details["PhoneNumber"],
                Email = details["Email"],
                BankAccountNumber = details["BankAccountNumber"],
                DateOfBirth = Convert.ToDateTime(details["DateOfBirth"])
            };
            using var res = await _customerApiDriver.UpdateCustomerAsync(command);
        }


        [When(@"user send a command to delete existing customer with email of ""([^""]*)""")]
        public async Task WhenUserSendACommandToDeleteExistingCustomerWithEmailOf(string p0)
        {
            var command = new DeleteCustomerCommand()
            {
                Email = p0
            };

            using var res = await _customerApiDriver.DeleteCustomerAsync(command);
        }

        [Then(@"user send a query to get all customers and receive ""([^""]*)"" record of customer")]
        public async Task ThenUserSendAQueryToGetAllCustomersAndReceiveRecordOfCustomer(string p0)
        {

            var query = new GetAllCustomerQuery();
            using var res = await _customerApiDriver.GetAllCustomersAsync(query);
            var response = (await res.Content.ReadAsStringAsync()).FromJson<ApiResponse<IEnumerable<CustomerQueryModel>>>();
            response.Result.Count().Should().Be(0);
        }


        //private WebApplicationFactory<Program> InitializeWebAppFactory(
        //    Action<IServiceCollection>? configureServices = null,
        //    Action<IServiceScope>? configureServiceScope = null)
        //{
        //    return new WebApplicationFactory<Program>()
        //        .WithWebHostBuilder(hostBuilder =>
        //        {
        //            hostBuilder.UseSetting("ConnectionStrings:SqlConnection", _msSqlFixture.Container.GetConnectionString());
        //            hostBuilder.UseSetting("ConnectionStrings:NoSqlConnection", _mongoDbFixture.Container.GetConnectionString());

        //            hostBuilder.UseEnvironment(Environments.Development);
        //            hostBuilder.ConfigureAppConfiguration((context, config) =>
        //            {
        //            });

        //            hostBuilder.ConfigureServices(services =>
        //            {
        //                services.RemoveAll<WriteDbContext>();
        //                services.RemoveAll<DbContextOptions<WriteDbContext>>();
        //                services.RemoveAll<EventStoreDbContext>();
        //                services.RemoveAll<DbContextOptions<EventStoreDbContext>>();
        //                services.RemoveAll<ISynchronizeDb>();

        //                services.AddDbContext<WriteDbContext>(
        //                    options => options.UseSqlServer(_msSqlFixture.Container.GetConnectionString())
        //                );

        //                services.AddDbContext<EventStoreDbContext>(options =>
        //                {
        //                    options.UseSqlServer(_msSqlFixture.Container.GetConnectionString());
        //                });

        //                services.AddSingleton<IReadDbContext, NoSqlDbContext>();
        //                services.AddSingleton<ISynchronizeDb, NoSqlDbContext>();

        //                configureServices?.Invoke(services);

        //                using var serviceProvider = services.BuildServiceProvider(true);
        //                using var serviceScope = serviceProvider.CreateScope();

        //                var writeDbContext = serviceScope.ServiceProvider.GetRequiredService<WriteDbContext>();
        //                writeDbContext.Database.EnsureCreated();

        //                //services.AddSingleton(_ => Substitute.For<EventStoreDbContext>());
        //                var eventStoreDbContext = serviceScope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
        //                eventStoreDbContext.Database.EnsureCreated();

        //                configureServiceScope?.Invoke(serviceScope);

        //                writeDbContext.Dispose();
        //                //eventStoreDbContext.Dispose();
        //            });



        //        });
        //}

        //private static WebApplicationFactoryClientOptions CreateClientOptions() => new WebApplicationFactoryClientOptions { AllowAutoRedirect = false };
    }




}

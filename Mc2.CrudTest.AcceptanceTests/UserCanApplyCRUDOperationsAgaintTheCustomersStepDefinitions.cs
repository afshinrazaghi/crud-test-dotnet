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
using TechTalk.SpecFlow.Assist;
using FluentAssertions.Execution;

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
            CreateCustomerCommand command = table.CreateInstance<CreateCustomerCommand>();
            ApiResponse<Presentation.Application.Features.Customers.Responses.CreatedCustomerResponse> res = await _customerApiDriver.CreateCustomerAsync(command, CancellationToken.None);
            using (new AssertionScope())
            {
                res.Should().NotBeNull();
                res.Success.Should().BeTrue();
            }
        }

        [Then(@"user send query and receive ""([^""]*)"" record of customer with following information")]
        public async Task ThenUserSendQueryAndReceiveRecordOfCustomerWithFollowingInformation(string p0, Table table)
        {
            GetAllCustomerQuery query = new GetAllCustomerQuery();
            ApiResponse<IEnumerable<CustomerQueryModel>> response = await _customerApiDriver.GetAllCustomersAsync(CancellationToken.None);
            CustomerQueryModel customer = table.CreateInstance<CustomerQueryModel>();
            response.Result.Where(x =>
                    x.Equals(customer)).Count().ToString().Should().Be(p0);
        }

        [When(@"user send command to update existing customer with email of ""([^""]*)"" with following information")]
        public async Task WhenUserSendCommandToUpdateExistingCustomerWithEmailOfWithFollowingInformation(string p0, Table table)
        {
            UpdateCustomerCommand command = table.CreateInstance<UpdateCustomerCommand>();
            command.OriginalEmail = p0;
            ApiResponse res = await _customerApiDriver.UpdateCustomerAsync(command, CancellationToken.None);
            using (new AssertionScope())
            {
                res.Should().NotBeNull();
                res.Success.Should().BeTrue();
            }
        }


        [When(@"user send a command to delete existing customer with email of ""([^""]*)""")]
        public async Task WhenUserSendACommandToDeleteExistingCustomerWithEmailOf(string p0)
        {
            DeleteCustomerCommand command = new DeleteCustomerCommand() { Email = p0 };
            ApiResponse res = await _customerApiDriver.DeleteCustomerAsync(command, CancellationToken.None);
            using (new AssertionScope())
            {
                res.Should().NotBeNull();
                res.Success.Should().BeTrue();
            }
        }

        [Then(@"user send a query to get all customers and receive ""([^""]*)"" record of customer")]
        public async Task ThenUserSendAQueryToGetAllCustomersAndReceiveRecordOfCustomer(string p0)
        {
            ApiResponse<IEnumerable<CustomerQueryModel>> response = await _customerApiDriver.GetAllCustomersAsync(CancellationToken.None);
            response.Result.Count().Should().Be(0);
        }
    }




}

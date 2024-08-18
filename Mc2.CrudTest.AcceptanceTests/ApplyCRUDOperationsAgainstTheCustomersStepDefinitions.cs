using FluentAssertions;
using FluentAssertions.Execution;
using Mc2.CrudTest.AcceptanceTests.Drivers;
using Mc2.CrudTest.AcceptanceTests.Fixtures;
using Mc2.CrudTest.Presentation.Application.Common.Interfaces.Abstractions;
using Mc2.CrudTest.Presentation.Application.Common.Interfaces.Persistence.Query;
using Mc2.CrudTest.Presentation.Application.Features.Customers.Commands;
using Mc2.CrudTest.Presentation.Application.Features.Customers.Queries;
using Mc2.CrudTest.Presentation.Application.Features.Customers.Responses;
using Mc2.CrudTest.Presentation.Application.Models;
using Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate;
using Mc2.CrudTest.Presentation.Domain.Factories;
using Mc2.CrudTest.Presentation.Domain.ValueObjects;
using Mc2.CrudTest.Presentation.Infrastructure.Command;
using Mc2.CrudTest.Presentation.Infrastructure.Command.Context;
using Mc2.CrudTest.Presentation.Infrastructure.Command.Persistence;
using Mc2.CrudTest.Presentation.Infrastructure.Query.Context;
using Mc2.CrudTest.Presentation.Shared.Extensions;
using Mc2.CrudTest.Presentation.Shared.Models;
using Mc2.CrudTest.Presentation.Shared.SharedKernel.Command;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Mc2.CrudTest.AcceptanceTests
{
    [Binding]
    public class ApplyCRUDOperationsAgainstTheCustomersStepDefinitions
    {
        private readonly CustomerApiDriver _customerApiDriver;
        List<CustomerQueryModel> existingCustomers = new List<CustomerQueryModel>();
        public ApplyCRUDOperationsAgainstTheCustomersStepDefinitions(CustomerApiDriver customerApiDriver)
        {
            _customerApiDriver = customerApiDriver;
        }

        [Given(@"platform has (.*) record of customer")]
        public async Task GivenPlatformHasRecordOfCustomer(int p0)
        {
            ApiResponse<IEnumerable<CustomerQueryModel>> response = await _customerApiDriver.GetAllCustomersAsync(CancellationToken.None);
            using (new AssertionScope())
            {
                response.Should().NotBeNull();
                response.Success.Should().BeTrue();
                response.Result.Count().Should().Be(p0);
            }
        }

        [When(@"user send command to create new customer with the following information")]
        public async Task WhenUserSendCommandToCreateNewCustomerWithTheFollowingInformation(Table table)
        {
            CreateCustomerCommand command = table.CreateInstance<CreateCustomerCommand>();
            ApiResponse<CreatedCustomerResponse> res = await _customerApiDriver.CreateCustomerAsync(command, CancellationToken.None);
            using (new AssertionScope())
            {
                res.Should().NotBeNull();
                res.Success.Should().BeTrue();
            }
        }

        [Then(@"user send query and receive (.*) record of customer with the following information")]
        public async Task ThenUserSendQueryAndReceiveRecordOfCustomerWithTheFollowingInformation(int p0, Table table)
        {
            ApiResponse<IEnumerable<CustomerQueryModel>> response = await _customerApiDriver.GetAllCustomersAsync(CancellationToken.None);
            CustomerQueryModel model = table.CreateInstance<CustomerQueryModel>();

            using (new AssertionScope())
            {
                response.Should().NotBeNull();
                int count = response.Result.Count(m => m.Equals(model));
                count.Should().Be(p0);
            }

        }

        [Given(@"a customer with following details exists")]
        public async Task GivenACustomerWithFollowingDetailsExists(Table table)
        {
            // Insert Into database
            CreateCustomerCommand command = table.CreateInstance<CreateCustomerCommand>();

            ApiResponse<CreatedCustomerResponse> createResponse = await _customerApiDriver.CreateCustomerAsync(command, CancellationToken.None);
            using (new AssertionScope())
            {
                createResponse.Should().NotBeNull();
                createResponse.Success.Should().BeTrue();
            }

            // Check record exists
            CustomerQueryModel model = table.CreateInstance<CustomerQueryModel>();
            ApiResponse<IEnumerable<CustomerQueryModel>> getAllCustomersResponse = await _customerApiDriver.GetAllCustomersAsync(CancellationToken.None);
            using (new AssertionScope())
            {
                getAllCustomersResponse.Should().NotBeNull();
                getAllCustomersResponse.Success.Should().BeTrue();
                getAllCustomersResponse.Result.Count().Should().Be(1);
                getAllCustomersResponse.Result.Where(x => x.Equals(model)).Count().Should().Be(1);
            }
        }

        [When(@"user send command to update customer with email of ""([^""]*)"" with following information")]
        public async Task WhenUserSendCommandToUpdateCustomerWithEmailOfWithFollowingInformation(string p0, Table table)
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

        [When(@"user send command to delete existing customer with email of ""([^""]*)""")]
        public async Task WhenUserSendCommandToDeleteExistingCustomerWithEmailOf(string p0)
        {
            DeleteCustomerCommand command = new DeleteCustomerCommand() { Email = p0 };
            ApiResponse res = await _customerApiDriver.DeleteCustomerAsync(command, CancellationToken.None);
        }


        [Then(@"user send query to get all customers and receive (.*) record of customer")]
        public async Task ThenUserSendQueryToGetAllCustomersAndReceiveRecordOfCustomer(int p0)
        {
            ApiResponse<IEnumerable<CustomerQueryModel>> response = await _customerApiDriver.GetAllCustomersAsync(CancellationToken.None);
            using (new AssertionScope())
            {
                response.Should().NotBeNull();
                response.Success.Should().BeTrue();
                response.Result.Count().Should().Be(p0);
            }
        }

        [Given(@"customers with the following details are exists in database")]
        public async Task GivenCustomersWithTheFollowingDetailsAreExistsInDatabase(Table table)
        {
            ApiResponse<IEnumerable<CustomerQueryModel>> response = await _customerApiDriver.GetAllCustomersAsync(CancellationToken.None);
            CustomerQueryModel customer = table.CreateInstance<CustomerQueryModel>();
            response.Result.Where(x =>
                   x.Equals(customer)).Count().Should().Be(1);
        }

        [When(@"user send query to get all customers")]
        public async Task WhenUserSendQueryToGetAllCustomers()
        {
            ApiResponse<IEnumerable<CustomerQueryModel>> response = await _customerApiDriver.GetAllCustomersAsync(CancellationToken.None);
            if (response.Success)
                existingCustomers = response.Result.ToList();
        }

        [Then(@"user should receive customers with the following information")]
        public void ThenUserShouldReceiveCustomersWithTheFollowingInformation(Table table)
        {
            existingCustomers.Count().Should().Be(table.Rows.Count);
            IEnumerable<CustomerQueryModel> customers = table.CreateSet<CustomerQueryModel>();
            foreach (CustomerQueryModel customer in customers)
            {
                existingCustomers.FirstOrDefault(x => x.Equals(customer)).Should().NotBeNull();
            }
        }
    }
}

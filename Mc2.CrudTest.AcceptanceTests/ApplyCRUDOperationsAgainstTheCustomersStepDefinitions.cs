using FluentAssertions;
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
using TechTalk.SpecFlow;

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
        public async void GivenPlatformHasRecordOfCustomer(int p0)
        {
            var query = new GetAllCustomerQuery();
            var response = await _customerApiDriver.GetAllCustomersAsync(query);
            var res = (await response.Content.ReadAsStringAsync()).FromJson<ApiResponse<IEnumerable<CustomerQueryModel>>>();
            res.Success.Should().BeTrue();
            res.Result.Count().Should().Be(p0);
        }

        [When(@"user send command to create new customer with the following information")]
        public async Task WhenUserSendCommandToCreateNewCustomerWithTheFollowingInformation(Table table)
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

        [Then(@"user send query and receive (.*) record of customer with the following information")]
        public async Task ThenUserSendQueryAndReceiveRecordOfCustomerWithTheFollowingInformation(int p0, Table table)
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
                    x.BankAccountNumber == details["BankAccountNumber"]).Count().Should().Be(p0);
        }

        [Given(@"a customer with following details exists")]
        public async Task GivenACustomerWithFollowingDetailsExists(Table table)
        {
            // Insert Into database
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

            using var createCustomerRes = await _customerApiDriver.CreateCustomerAsync(command);
            var createCustomerResult = (await createCustomerRes.Content.ReadAsStringAsync()).FromJson<ApiResponse<CreatedCustomerResponse>>();
            createCustomerResult.Should().NotBeNull();
            createCustomerResult.Success.Should().BeTrue();

            // Check record exists

            var query = new GetAllCustomerQuery();
            using var getAllCustomerRes = await _customerApiDriver.GetAllCustomersAsync(query);
            var getAllCustomersResponse = (await getAllCustomerRes.Content.ReadAsStringAsync()).FromJson<ApiResponse<IEnumerable<CustomerQueryModel>>>();
            getAllCustomersResponse.Should().NotBeNull();
            getAllCustomersResponse.Success.Should().BeTrue();
            getAllCustomersResponse.Result.Count().Should().Be(1);
            getAllCustomersResponse.Result.Where(x =>
                    x.FirstName == details["FirstName"] &&
                    x.LastName == details["LastName"] &&
                    x.PhoneNumber == details["PhoneNumber"] &&
                    x.Email == details["Email"] &&
                    x.DateOfBirth == Convert.ToDateTime(details["DateOfBirth"]) &&
                    x.BankAccountNumber == details["BankAccountNumber"]).Count().Should().Be(1);
        }

        [When(@"user send command to update customer with email of ""([^""]*)"" with following information")]
        public async Task WhenUserSendCommandToUpdateCustomerWithEmailOfWithFollowingInformation(string p0, Table table)
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

        [When(@"user send command to delete existing customer with email of ""([^""]*)""")]
        public async Task WhenUserSendCommandToDeleteExistingCustomerWithEmailOf(string p0)
        {
            var command = new DeleteCustomerCommand()
            {
                Email = p0
            };

            using var res = await _customerApiDriver.DeleteCustomerAsync(command);
        }


        [Then(@"user send query to get all customers and receive (.*) record of customer")]
        public async Task ThenUserSendQueryToGetAllCustomersAndReceiveRecordOfCustomer(int p0)
        {
            var query = new GetAllCustomerQuery();
            var response = await _customerApiDriver.GetAllCustomersAsync(query);
            var res = (await response.Content.ReadAsStringAsync()).FromJson<ApiResponse<IEnumerable<CustomerQueryModel>>>();
            res.Should().NotBeNull();
            res.Success.Should().BeTrue();
            res.Result.Count().Should().Be(p0);
        }

        [Given(@"customers with the following details are exists in database")]
        public async Task GivenCustomersWithTheFollowingDetailsAreExistsInDatabase(Table table)
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
                    x.BankAccountNumber == details["BankAccountNumber"]).Count().Should().Be(1);
        }

        [When(@"user send query to get all customers")]
        public async Task WhenUserSendQueryToGetAllCustomers()
        {
            var query = new GetAllCustomerQuery();
            var response = await _customerApiDriver.GetAllCustomersAsync(query);
            var res = (await response.Content.ReadAsStringAsync()).FromJson<ApiResponse<IEnumerable<CustomerQueryModel>>>();
            if (res.Success)
            {
                existingCustomers = res.Result.ToList();
            }
        }

        [Then(@"user should receive customers with the following information")]
        public void ThenUserShouldReceiveCustomersWithTheFollowingInformation(Table table)
        {
            existingCustomers.Count().Should().Be(table.Rows.Count);
            foreach (var row in table.Rows)
            {
                var customer = existingCustomers.FirstOrDefault(c =>
                     c.FirstName == row["FirstName"] &&
                     c.LastName == row["LastName"] &&
                     c.DateOfBirth == Convert.ToDateTime(row["DateOfBirth"]) &&
                     c.PhoneNumber == row["PhoneNumber"] &&
                     c.Email == row["Email"] &&
                     c.BankAccountNumber == row["BankAccountNumber"]
                 );
                customer.Should().NotBeNull();
            }
        }
    }
}

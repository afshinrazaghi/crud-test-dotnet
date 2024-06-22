using FluentAssertions;
using Mc2.CrudTest.Presentation.Application.Features.Customers.CommandHandlers;
using Mc2.CrudTest.Presentation.Application.Features.Customers.Commands;
using Mc2.CrudTest.Presentation.Application.Features.Customers.Queries;
using Mc2.CrudTest.Presentation.Application.Features.Customers.QueryHandlers;
using Mc2.CrudTest.Presentation.Application.Models;
using Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate;
using NUnit.Framework;
using System;
using TechTalk.SpecFlow;

namespace Mc2.CrudTest.AcceptanceTests
{
    [Binding]
    public class CustomerManagerStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly CreateCustomerCommandHandler _createCustomerCommandHandler;
        private readonly GetCustomerByIdQueryHandler _getCustomerByIdQueryHandler;
        private CreateCustomerCommand _createCustomerCommand;
        private CustomerQueryModel _retrievedCustomer;
        private Guid createdCustomerId;

        public CustomerManagerStepDefinitions(ScenarioContext scenarioContext, CreateCustomerCommandHandler createCustomerCommandHandler, GetCustomerByIdQueryHandler getCustomerQueryHandler)
        {
            _scenarioContext = scenarioContext;
            _createCustomerCommandHandler = createCustomerCommandHandler;
            _getCustomerByIdQueryHandler = getCustomerQueryHandler;
        }

        [Given(@"I have a new customer with the following details")]
        public void GivenIHaveANewCustomerWithTheFollowingDetails(Table table)
        {
            var details = table.Rows.First();
            _createCustomerCommand = new CreateCustomerCommand()
            {
                FirstName = details["FirstName"],
                LastName = details["LastName"],
                DateOfBirth = DateTime.Parse(details["DateOfBirth"]),
                PhoneNumber = details["PhoneNumber"],
                Email = details["Email"],
                BankAccountNumber = details["BankAccountNumber"]
            };
        }

        [When(@"I create the customer")]
        public async void WhenICreateTheCustomer()
        {
            var res = await _createCustomerCommandHandler.Handle(_createCustomerCommand, default);
            if (res.IsSuccess)
                createdCustomerId = res.Value.Id;
        }

        [Then(@"the customer should be saved in the system")]
        public async void ThenTheCustomerShouldBeSavedInTheSystem()
        {
            var query = new GetCustomerByIdQuery(createdCustomerId);
            var result = await _getCustomerByIdQueryHandler.Handle(query, default);
            result.IsSuccess.Should().Be(true);
            _retrievedCustomer = result.Value;
            _retrievedCustomer.Should().NotBeNull();
        }

        [Then(@"I should be able to retrieve the customer with the same details")]
        public void ThenIShouldBeAbleToRetrieveTheCustomerWithTheSameDetails()
        {
            _createCustomerCommand.FirstName.Should().Be(_retrievedCustomer.FirstName);
            _createCustomerCommand.LastName.Should().Be(_retrievedCustomer.LastName);
            _createCustomerCommand.DateOfBirth.Should().Be(_retrievedCustomer.DateOfBirth);
            _createCustomerCommand.PhoneNumber.Should().Be(_retrievedCustomer.PhoneNumber);
            _createCustomerCommand.Email.Should().Be(_retrievedCustomer.Email);
            _createCustomerCommand.BankAccountNumber.Should().Be(_retrievedCustomer.BankAccountNumber);
        }
    }
}

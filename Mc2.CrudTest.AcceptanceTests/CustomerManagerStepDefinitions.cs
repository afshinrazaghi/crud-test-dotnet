using FluentAssertions;
using Mc2.CrudTest.AcceptanceTests.Fixtures;
using Mc2.CrudTest.Presentation.Application.Features.Customers.CommandHandlers;
using Mc2.CrudTest.Presentation.Application.Features.Customers.Commands;
using Mc2.CrudTest.Presentation.Application.Features.Customers.Queries;
using Mc2.CrudTest.Presentation.Application.Features.Customers.QueryHandlers;
using Mc2.CrudTest.Presentation.Application.Models;
using Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate;
using Mc2.CrudTest.Presentation.Domain.Factories;
using Mc2.CrudTest.Presentation.Domain.ValueObjects;
using Mc2.CrudTest.Presentation.Infrastructure.Command;
using Mc2.CrudTest.Presentation.Infrastructure.Command.Persistence;
using Mc2.CrudTest.Presentation.Infrastructure.Query.Context;
using Mc2.CrudTest.Presentation.Infrastructure.Query.Persistence;
using Mc2.CrudTest.Presentation.Shared.SharedKernel.Command;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.ComponentModel.DataAnnotations;
using TechTalk.SpecFlow;

namespace Mc2.CrudTest.AcceptanceTests
{
    [Binding]
    public class CustomerManagerStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly EfSQLiteFixture _fixture;
        private readonly MongoFixture _mongoFixture;

        private CreateCustomerCommand _createCustomerCommand;
        private UpdateCustomerCommand _updateCustomerCommand;
        private CustomerQueryModel _retrievedCustomer;
        private Customer _existingCustomer;
        private Customer _updatedCustomer;
        private Guid createdCustomerId;
        private readonly CreateCustomerCommandValidator _createCustomerCommandValidator = new CreateCustomerCommandValidator();
        private readonly GetCustomerByIdQueryValidator _getCustomerByIdQueryValidator = new GetCustomerByIdQueryValidator();
        private readonly UpdateCustomerCommandValidator _updateCustomerCommandValidator = new UpdateCustomerCommandValidator();

        public CustomerManagerStepDefinitions(ScenarioContext scenarioContext, EfSQLiteFixture fixture, MongoFixture mongoFixture)
        {
            _scenarioContext = scenarioContext;
            _fixture = fixture;
            _mongoFixture = mongoFixture;
        }

        #region Create
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
            var unitOfWork = new UnitOfWork(
                _fixture.Context,
                Substitute.For<IEventStoreRepository>(),
                Substitute.For<IMediator>(),
                Substitute.For<ILogger<UnitOfWork>>());

            var repository = new CustomerWriteOnlyRepository(_fixture.Context);
            var createCustomerCommandHandler = new CreateCustomerCommandHandler(_createCustomerCommandValidator, repository, unitOfWork);

            var res = await createCustomerCommandHandler.Handle(_createCustomerCommand, default);
            if (res.IsSuccess)
                createdCustomerId = res.Value.Id;
        }

        [Then(@"the customer should be saved in the system")]
        public async void ThenTheCustomerShouldBeSavedInTheSystem()
        {

            var query = new GetCustomerByIdQuery(createdCustomerId);
            var customerReadonlyRepository = new CustomerReadOnlyRepository(_mongoFixture.Context);

            var getCustomerByIdQueryHandler = new GetCustomerByIdQueryHandler(customerReadonlyRepository, _getCustomerByIdQueryValidator);
            var result = await getCustomerByIdQueryHandler.Handle(query, default);
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
        #endregion

        #region Update
        [Given(@"an existing customer with following details")]
        public async void GivenAnExistingCustomerWithFollowingDetails(Table table)
        {
            var details = table.Rows.First();
            _existingCustomer = CustomerFactory.Create(
                    details["FirstName"],
                    details["LastName"],
                    Convert.ToDateTime(details["DateOfBirth"]),
                    details["PhoneNumber"],
                    details["Email"],
                    details["BankAccountNumber"]
            );

            var customerWriteOnlyRepository = new CustomerWriteOnlyRepository(_fixture.Context);
            customerWriteOnlyRepository.Add(_existingCustomer);
            await _fixture.Context.SaveChangesAsync();
            _fixture.Context.ChangeTracker.Clear();
        }

        [When(@"I update the customer's details with the following information")]
        public async void WhenIUpdateTheCustomersDetailsWithTheFollowingInformation(Table table)
        {
            var details = table.Rows.First();

            _updatedCustomer = CustomerFactory.Create(
                    details["FirstName"],
                    details["LastName"],
                    Convert.ToDateTime(details["DateOfBirth"]),
                    details["PhoneNumber"],
                    details["Email"],
                    details["BankAccountNumber"]
            );

            _updateCustomerCommand = new UpdateCustomerCommand()
            {
                Id = _existingCustomer.Id,
                FirstName = details["FirstName"],
                LastName = details["LastName"],
                DateOfBirth = DateTime.Parse(details["DateOfBirth"]),
                PhoneNumber = details["PhoneNumber"],
                Email = details["Email"],
                BankAccountNumber = details["BankAccountNumber"]
            };

            var unitOfWork = new UnitOfWork(
                _fixture.Context,
                Substitute.For<IEventStoreRepository>(),
                Substitute.For<IMediator>(),
                Substitute.For<ILogger<UnitOfWork>>());

            var repository = new CustomerWriteOnlyRepository(_fixture.Context);

            var updateCustomerCommandHandler = new UpdateCustomerCommandHandler(_updateCustomerCommandValidator, repository, unitOfWork);

            var result = await updateCustomerCommandHandler.Handle(_updateCustomerCommand, default);
            result.IsSuccess.Should().Be(true);
        }

        [Then(@"the customer's details should be updated successfully")]
        public async void ThenTheCustomersDetailsShouldBeUpdatedSuccessfully()
        {
            var query = new GetCustomerByIdQuery(_existingCustomer.Id);
            var customerReadonlyRepository = new CustomerReadOnlyRepository(_mongoFixture.Context);

            var getCustomerByIdQueryHandler = new GetCustomerByIdQueryHandler(customerReadonlyRepository, _getCustomerByIdQueryValidator);
            var result = await getCustomerByIdQueryHandler.Handle(query, default);
            result.IsSuccess.Should().Be(true);
        }

        [Then(@"the customer should have the following updated details")]
        public void ThenTheCustomerShouldHaveTheFollowingUpdatedDetails(Table table)
        {
            _updateCustomerCommand.FirstName.Should().Be(_updatedCustomer.FirstName);
            _updateCustomerCommand.LastName.Should().Be(_updatedCustomer.LastName);
            _updateCustomerCommand.DateOfBirth.Should().Be(_updatedCustomer.DateOfBirth);
            _updateCustomerCommand.PhoneNumber.Should().Be(_updatedCustomer.PhoneNumber.Value);
            _updateCustomerCommand.Email.Should().Be(_updatedCustomer.Email.Value);
            _updateCustomerCommand.BankAccountNumber.Should().Be(_updatedCustomer.BankAccountNumber.Value);
        }

        #endregion
    }
}

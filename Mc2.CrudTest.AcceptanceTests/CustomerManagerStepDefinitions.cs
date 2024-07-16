using Ardalis.Result;
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
using Mc2.CrudTest.Presentation.Infrastructure.EventHandlers;
using Mc2.CrudTest.Presentation.Infrastructure.Query.Context;
using Mc2.CrudTest.Presentation.Infrastructure.Query.Persistence;
using Mc2.CrudTest.Presentation.Shared.SharedKernel.Command;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.CommonModels;
using Xunit;
using System.Linq;

namespace Mc2.CrudTest.AcceptanceTests
{
    [Binding]
    public class CustomerManagerStepDefinitions : IClassFixture<EFSQLFixture>
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly EFSQLFixture _fixture;
        private readonly MongoFixture _mongoFixture;

        private CreateCustomerCommand _createCustomerCommand;
        private UpdateCustomerCommand _updateCustomerCommand;
        private Customer? _retrievedCustomer;
        private Customer _existingCustomer;
        private Customer _updatedCustomer;
        private Customer _targetDeleteCustomer;
        private Guid createdCustomerId;
        private Ardalis.Result.Result _updateCustomerResult;
        private readonly CreateCustomerCommandValidator _createCustomerCommandValidator = new CreateCustomerCommandValidator();
        private readonly GetCustomerByIdQueryValidator _getCustomerByIdQueryValidator = new GetCustomerByIdQueryValidator();
        private readonly UpdateCustomerCommandValidator _updateCustomerCommandValidator = new UpdateCustomerCommandValidator();
        private readonly DeleteCustomerCommandValidator _deleteCustomerCommandValidator = new DeleteCustomerCommandValidator();
        Ardalis.Result.Result<IEnumerable<CustomerQueryModel>> getAllCustomersResult;


        public CustomerManagerStepDefinitions(ScenarioContext scenarioContext, EFSQLFixture fixture, MongoFixture mongoFixture)
        {
            _scenarioContext = scenarioContext;
            _fixture = fixture;
            _fixture.InitializeAsync().Wait();
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
            var repository = new CustomerWriteOnlyRepository(_fixture.Context);
            _retrievedCustomer = await repository.GetByIdAsync(createdCustomerId);
            _retrievedCustomer.Should().NotBeNull();
            //var query = new GetCustomerByIdQuery(createdCustomerId);
            //var customerReadonlyRepository = new CustomerReadOnlyRepository(_mongoFixture.Context);

            //var getCustomerByIdQueryHandler = new GetCustomerByIdQueryHandler(customerReadonlyRepository, _getCustomerByIdQueryValidator);
            //var result = await getCustomerByIdQueryHandler.Handle(query, default);
            //result.IsSuccess.Should().Be(true);
            //_retrievedCustomer = result.Value;
            //_retrievedCustomer.Should().NotBeNull();
        }

        [Then(@"I should be able to retrieve the customer with the same details")]
        public void ThenIShouldBeAbleToRetrieveTheCustomerWithTheSameDetails()
        {
            _createCustomerCommand.FirstName.Should().Be(_retrievedCustomer!.FirstName);
            _createCustomerCommand.LastName.Should().Be(_retrievedCustomer!.LastName);
            _createCustomerCommand.DateOfBirth.Should().Be(_retrievedCustomer!.DateOfBirth);
            _createCustomerCommand.PhoneNumber.Should().Be(_retrievedCustomer!.PhoneNumber.Value);
            _createCustomerCommand.Email.Should().Be(_retrievedCustomer!.Email.Value);
            _createCustomerCommand.BankAccountNumber.Should().Be(_retrievedCustomer!.BankAccountNumber.Value);
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

            //_updatedCustomer = CustomerFactory.Create(
            //        details["FirstName"],
            //        details["LastName"],
            //        Convert.ToDateTime(details["DateOfBirth"]),
            //        details["PhoneNumber"],
            //        details["Email"],
            //        details["BankAccountNumber"]
            //);

            _updateCustomerCommand = new UpdateCustomerCommand()
            {
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

            _updateCustomerResult = await updateCustomerCommandHandler.Handle(_updateCustomerCommand, default);

        }

        [Then(@"the customer's details should be updated successfully")]
        public void ThenTheCustomersDetailsShouldBeUpdatedSuccessfully()
        {
            _updateCustomerResult.IsSuccess.Should().Be(true);

            //var query = new GetCustomerByIdQuery(_existingCustomer.Id);
            //var customerReadonlyRepository = new CustomerReadOnlyRepository(_mongoFixture.Context);

            //var getCustomerByIdQueryHandler = new GetCustomerByIdQueryHandler(customerReadonlyRepository, _getCustomerByIdQueryValidator);
            //var result = await getCustomerByIdQueryHandler.Handle(query, default);
            //result.IsSuccess.Should().Be(true);
        }

        [Then(@"the customer should have the following updated details")]
        public async void ThenTheCustomerShouldHaveTheFollowingUpdatedDetails(Table table)
        {
            var repository = new CustomerWriteOnlyRepository(_fixture.Context);
            _updatedCustomer = (await repository.GetByEmailAsync(Email.Create(_updateCustomerCommand.Email).Value))!;

            _updateCustomerCommand.FirstName.Should().Be(_updatedCustomer.FirstName);
            _updateCustomerCommand.LastName.Should().Be(_updatedCustomer.LastName);
            _updateCustomerCommand.DateOfBirth.Should().Be(_updatedCustomer.DateOfBirth);
            _updateCustomerCommand.PhoneNumber.Should().Be(_updatedCustomer.PhoneNumber.Value);
            _updateCustomerCommand.Email.Should().Be(_updatedCustomer.Email.Value);
            _updateCustomerCommand.BankAccountNumber.Should().Be(_updatedCustomer.BankAccountNumber.Value);
        }

        #endregion

        #region Delete
        [Given(@"a customer with the following details")]
        public async void GivenACustomerWithTheFollowingDetails(Table table)
        {
            var details = table.Rows.First();
            _targetDeleteCustomer = CustomerFactory.Create(
                    details["FirstName"],
                    details["LastName"],
                    Convert.ToDateTime(details["DateOfBirth"]),
                    details["PhoneNumber"],
                    details["Email"],
                    details["BankAccountNumber"]
            );

            var customerWriteOnlyRepository = new CustomerWriteOnlyRepository(_fixture.Context);
            customerWriteOnlyRepository.Add(_targetDeleteCustomer);
            await _fixture.Context.SaveChangesAsync();
            _fixture.Context.ChangeTracker.Clear();
        }


        [When(@"I delete the customer with target Id")]
        public async void WhenIDeleteTheCustomerWithTargetId()
        {
            var command = new DeleteCustomerCommand()
            {
                Email = _targetDeleteCustomer.Email
            };
            var repository = new CustomerWriteOnlyRepository(_fixture.Context);
            var unitOfWork = new UnitOfWork(
                    _fixture.Context,
                    Substitute.For<IEventStoreRepository>(),
                    Substitute.For<IMediator>(),
                    Substitute.For<ILogger<UnitOfWork>>()
             );

            var handler = new DeleteCustomerCommandHandler(
                _deleteCustomerCommandValidator,
                repository,
                unitOfWork
             );

            await handler.Handle(command, CancellationToken.None);
        }

        [Then(@"the customer with target Id should not exist")]
        public async void ThenTheCustomerWithTargetIdShouldNotExist()
        {
            var customerWriteOnlyRepository = new CustomerWriteOnlyRepository(_fixture.Context);
            var deletedCustomer = await customerWriteOnlyRepository.GetByIdAsync(_targetDeleteCustomer.Id);
            deletedCustomer.Should().BeNull();
        }


        #endregion

        #region Get All Customers
        [Given(@"the following customers exists")]
        public async Task GivenTheFollowingCustomersExists(Table table)
        {
            var repository = new CustomerWriteOnlyRepository(_fixture.Context);
            var unitOfWork = new UnitOfWork(
                _fixture.Context,
                Substitute.For<IEventStoreRepository>(),
                Substitute.For<IMediator>(),
                Substitute.For<ILogger<UnitOfWork>>()
            );


            List<Customer> customers = new List<Customer>();
            foreach (var row in table.Rows)
            {
                var customer = CustomerFactory.Create(
                    row["FirstName"],
                    row["LastName"],
                    Convert.ToDateTime(row["DateOfBirth"]),
                    row["PhoneNumber"],
                    row["Email"],
                    row["BankAccountNumber"]
                ).Value;

                customers.Add(customer);
            }

            await _fixture.Context.SaveChangesAsync();
            _fixture.Context.ChangeTracker.Clear();

            List<Task> tasks = new List<Task>();

            foreach (var customer in customers)
            {
                CustomerQueryModel model = new CustomerQueryModel(customer.Id, customer.FirstName, customer.LastName, customer.DateOfBirth, customer.PhoneNumber.Value, customer.Email.Value, customer.BankAccountNumber.Value);
                tasks.Add(_mongoFixture.Context.UpsertAsync(model, filter => filter.Id == model.Id));
            }

            await Task.WhenAll(tasks);
        }

        [When(@"I retrieve all customers")]
        public async void WhenIRetrieveAllCustomers()
        {
            var getAllCustomerQuery = new GetAllCustomerQuery();
            var repository = new CustomerReadOnlyRepository(_mongoFixture.Context);
            var handler = new GetAllCustomerQueryHandler(repository);
            getAllCustomersResult = await handler.Handle(getAllCustomerQuery, CancellationToken.None);

        }

        [Then(@"I should see the following customers")]
        public void ThenIShouldSeeTheFollowingCustomers(Table table)
        {
            getAllCustomersResult.Should().NotBeNull();
            getAllCustomersResult.Value.Should().NotBeNull().And.HaveCount(2);
        }

        #endregion
    }
}

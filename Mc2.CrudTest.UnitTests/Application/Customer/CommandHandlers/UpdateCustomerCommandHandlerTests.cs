using Bogus;
using FluentAssertions;
using Mc2.CrudTest.Presentation.Application.Features.Customers.CommandHandlers;
using Mc2.CrudTest.Presentation.Application.Features.Customers.Commands;
using Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate;
using Mc2.CrudTest.Presentation.Domain.Factories;
using Mc2.CrudTest.Presentation.Infrastructure.Command;
using Mc2.CrudTest.Presentation.Infrastructure.Command.Persistence;
using Mc2.CrudTest.Presentation.Shared.SharedKernel.Command;
using Mc2.CrudTest.UnitTests.Fixtures;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.UnitTests.Application.Customer.CommandHandlers
{
    public class UpdateCustomerCommandHandlerTests
    {
        private readonly UpdateCustomerCommandValidator _validator = new UpdateCustomerCommandValidator();
        private readonly EfSQLiteFixture _fixture;
        public UpdateCustomerCommandHandlerTests(EfSQLiteFixture fixture)
        {
            _fixture = fixture;
        }
        [Fact]
        public async void Handle_WhenCalledWithValidCustomer_ReturnsSuccess()
        {
            // Arrange
            var customer = new Faker<Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate.Customer>().
                CustomInstantiator(faker => CustomerFactory.Create(
                    faker.Person.FirstName,
                    faker.Person.LastName,
                    faker.Person.DateOfBirth,
                    faker.Random.Number(100000000, 999999999).ToString(),
                    faker.Person.Email,
                    BankAccountNumberFixture.Generate()
                )).Generate();

            var repository = new CustomerWriteOnlyRepository(_fixture.Context);
            repository.Add(customer);

            await _fixture.Context.SaveChangesAsync();
            _fixture.Context.ChangeTracker.Clear();

            var unitOfWork = new UnitOfWork(
                _fixture.Context,
                Substitute.For<IEventStoreRepository>(),
                Substitute.For<IMediator>(),
                Substitute.For<ILogger<UnitOfWork>>()
            );

            var command = new Faker<UpdateCustomerCommand>()
                .RuleFor(command => command.Id, customer.Id)
                .RuleFor(command => command.FirstName, faker => faker.Person.FirstName)
                .RuleFor(command => command.LastName, faker => faker.Person.LastName)
                .RuleFor(command => command.DateOfBirth, faker => faker.Person.DateOfBirth)
                .RuleFor(command => command.PhoneNumber, faker => faker.Random.Number(100000000, 999999999).ToString())
                .RuleFor(command => command.Email, faker => faker.Person.Email)
                .RuleFor(command => command.BankAccountNumber, faker => BankAccountNumberFixture.Generate())
                .Generate();

            var handler = new UpdateCustomerCommandHandler(_validator, repository, unitOfWork);

            // Act
            var res = await handler.Handle(command, CancellationToken.None);

            // Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeTrue();
            res.SuccessMessage.Should().Be("Updated successfully!");
        }

        [Fact]
        public async void Handle_WhenDuplicateEmail_ReturnsFail()
        {
            // Arrange
            var customers = new Faker<Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate.Customer>().
               CustomInstantiator(faker => CustomerFactory.Create(
                   faker.Person.FirstName,
                   faker.Person.LastName,
                   faker.Person.DateOfBirth,
                   faker.Random.Number(100000000, 999999999).ToString(),
                   faker.Person.Email,
                   BankAccountNumberFixture.Generate()
               )).Generate(2);


            var repository = new CustomerWriteOnlyRepository(_fixture.Context);
            foreach (var customer in customers)
            {
                repository.Add(customer);
            }

            await _fixture.Context.SaveChangesAsync();
            _fixture.Context.ChangeTracker.Clear();

            var command = new Faker<UpdateCustomerCommand>()
                .RuleFor(command => command.Id, customers[0].Id)
                .RuleFor(command => command.Email, _ => customers[1].Email.Value)
                .RuleFor(command => command.FirstName, faker => faker.Person.FirstName)
                .RuleFor(command => command.LastName, faker => faker.Person.LastName)
                .RuleFor(command => command.DateOfBirth, faker => faker.Person.DateOfBirth)
                .RuleFor(command => command.PhoneNumber, faker => faker.Random.Number(100000000, 999999999).ToString())
                .RuleFor(command => command.BankAccountNumber, faker => BankAccountNumberFixture.Generate())
                .Generate();

            var handler = new UpdateCustomerCommandHandler(
                _validator,
                repository,
                Substitute.For<IUnitOfWork>());

            // Act

            var res = await handler.Handle(command, CancellationToken.None);

            // Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeFalse();
            res.Errors.Should()
                .NotBeNullOrEmpty()
                .And.OnlyHaveUniqueItems()
                .And.Contain(errorMessage => errorMessage == "email already exists");
        }


        [Fact]
        public async void Handle_WhenDuplicateFirstNameAndLastNameAndDateOfBirth_ReturnFail()
        {
            // Arrange
            var customers = new Faker<Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate.Customer>().
               CustomInstantiator(faker => CustomerFactory.Create(
                   faker.Person.FirstName,
                   faker.Person.LastName,
                   faker.Person.DateOfBirth,
                   faker.Random.Number(100000000, 999999999).ToString(),
                   faker.Person.Email,
                   BankAccountNumberFixture.Generate()
               )).Generate(2);

            var repository = new CustomerWriteOnlyRepository(_fixture.Context);
            foreach (var customer in customers)
            {
                repository.Add(customer);
            }

            await _fixture.Context.SaveChangesAsync();
            _fixture.Context.ChangeTracker.Clear();

            var command = new Faker<UpdateCustomerCommand>()
                .RuleFor(command => command.Id, customers[0].Id)
                .RuleFor(command => command.Email, customers[0].Email.Value)
                .RuleFor(command => command.FirstName, customers[1].FirstName)
                .RuleFor(command => command.LastName, customers[1].LastName)
                .RuleFor(command => command.DateOfBirth, customers[1].DateOfBirth)
                .RuleFor(command => command.PhoneNumber, faker => faker.Random.Number(100000000, 999999999).ToString())
                .RuleFor(command => command.BankAccountNumber, faker => BankAccountNumberFixture.Generate())
                .Generate();


            var handler = new UpdateCustomerCommandHandler(
                _validator,
                repository,
                Substitute.For<IUnitOfWork>());

            // Act
            var res = await handler.Handle(command, CancellationToken.None);

            // Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeFalse();
            res.Errors.Should().NotBeNullOrEmpty()
                .And.OnlyHaveUniqueItems()
                .And.Contain(errorMessage => errorMessage == "customer with target info already exists");
        }

        [Fact]
        public async void Handle_WhenCustomerNotFound_ReturnFail()
        {
            // Arrange
            var command = new Faker<UpdateCustomerCommand>()
                .RuleFor(command => command.Id, faker => faker.Random.Guid())
                .RuleFor(command => command.FirstName, faker => faker.Person.FirstName)
                .RuleFor(command => command.LastName, faker => faker.Person.LastName)
                .RuleFor(command => command.DateOfBirth, faker => faker.Person.DateOfBirth)
                .RuleFor(command => command.Email, faker => faker.Person.Email)
                .RuleFor(command => command.PhoneNumber, faker => faker.Random.Number(100000000, 999999999).ToString())
                .RuleFor(command => command.BankAccountNumber, faker => BankAccountNumberFixture.Generate())
                .Generate();

            var repository = new CustomerWriteOnlyRepository(_fixture.Context);

            var handler = new UpdateCustomerCommandHandler(
                _validator,
                repository,
                Substitute.For<IUnitOfWork>()
            );

            // Act

            var res = await handler.Handle(command, CancellationToken.None);

            // Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeFalse();
            res.Errors.Should()
                .NotBeNullOrEmpty()
                .And.OnlyHaveUniqueItems()
                .And.Contain(errorMessage => errorMessage == $"No customer found by Id : {command.Id}");
        }


        [Fact]
        public async void Handle_WhenCommandInValid_ReturnsFail()
        {
            // Arrange
            var handler = new UpdateCustomerCommandHandler(
                _validator,
                Substitute.For<ICustomerWriteOnlyRepository>(),
                Substitute.For<IUnitOfWork>());
            // Act

            var res = await handler.Handle(new UpdateCustomerCommand(), CancellationToken.None);

            // Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeFalse();
            res.ValidationErrors.Should().NotBeNullOrEmpty().And.OnlyHaveUniqueItems();
        }


    }
}

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
    public class CreateCustomerCommandHandlerTests : IClassFixture<EfSQLiteFixture>
    {
        private readonly CreateCustomerCommandValidator _validator = new CreateCustomerCommandValidator();
        private readonly EfSQLiteFixture _fixture;

        public CreateCustomerCommandHandlerTests(EfSQLiteFixture fixture)
        {
            _fixture = fixture;
        }


        [Fact]
        public async void Handle_WhenCalledWithValidData_ReturnsSuccess()
        {
            // Arrange
            var command = new Faker<CreateCustomerCommand>("nl")
                .RuleFor(command => command.FirstName, faker => faker.Person.FirstName)
                .RuleFor(command => command.LastName, faker => faker.Person.LastName)
                .RuleFor(command => command.DateOfBirth, faker => faker.Person.DateOfBirth)
                .RuleFor(command => command.PhoneNumber, faker => faker.Random.Number(100000000, 999999999).ToString())
                .RuleFor(command => command.Email, faker => faker.Person.Email)
                .RuleFor(command => command.BankAccountNumber, faker => BankAccountNumberFixture.Generate())
                .Generate();

            var unitOfWork = new UnitOfWork(
                _fixture.Context,
                Substitute.For<IEventStoreRepository>(),
                Substitute.For<IMediator>(),
                Substitute.For<ILogger<UnitOfWork>>());

            var handler = new CreateCustomerCommandHandler(
                _validator,
                new CustomerWriteOnlyRepository(_fixture.Context),
                unitOfWork);


            // Act
            var res = await handler.Handle(command, CancellationToken.None);

            // Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeTrue();
            res.SuccessMessage.Should().Be("Successfully registered!");
            res.Value.Should().NotBeNull();
            res.Value.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async void Handle_WhenDuplicateEmailExists_ReturnsFail()
        {
            // Arrange

            var command = new Faker<CreateCustomerCommand>("nl")
               .RuleFor(command => command.FirstName, faker => faker.Person.FirstName)
               .RuleFor(command => command.LastName, faker => faker.Person.LastName)
               .RuleFor(command => command.DateOfBirth, faker => faker.Person.DateOfBirth)
               .RuleFor(command => command.PhoneNumber, faker => faker.Random.Number(100000000, 999999999).ToString())
               .RuleFor(command => command.Email, faker => faker.Person.Email)
               .RuleFor(command => command.BankAccountNumber, faker => BankAccountNumberFixture.Generate())
               .Generate();

            var repository = new CustomerWriteOnlyRepository(_fixture.Context);
            repository.Add(CustomerFactory.Create(
                command.FirstName,
                command.LastName,
                command.DateOfBirth,
                command.PhoneNumber,
                command.Email,
                command.BankAccountNumber
             ));

            await _fixture.Context.SaveChangesAsync();
            _fixture.Context.ChangeTracker.Clear();

            var handler = new CreateCustomerCommandHandler(
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
        public async void Handle_WhenCalled_WithDuplicateFirstNameLastNameAndDateOfBirth_ReturnsFail()
        {
            // Arrange

            var command = new Faker<CreateCustomerCommand>("nl")
               .RuleFor(command => command.FirstName, faker => faker.Person.FirstName)
               .RuleFor(command => command.LastName, faker => faker.Person.LastName)
               .RuleFor(command => command.DateOfBirth, faker => faker.Person.DateOfBirth)
               .RuleFor(command => command.PhoneNumber, faker => faker.Random.Number(100000000, 999999999).ToString())
               .RuleFor(command => command.Email, faker => faker.Person.Email)
               .RuleFor(command => command.BankAccountNumber, faker => BankAccountNumberFixture.Generate())
               .Generate();

            var repository = new CustomerWriteOnlyRepository(_fixture.Context);
            repository.Add(CustomerFactory.Create(
                command.FirstName,
                command.LastName,
                command.DateOfBirth,
                command.PhoneNumber,
                "afshin.razaghi.net@gmail.com",
                command.BankAccountNumber
             ));

            await _fixture.Context.SaveChangesAsync();
            _fixture.Context.ChangeTracker.Clear();

            var handler = new CreateCustomerCommandHandler(
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
                .And.Contain(errorMessage => errorMessage == "customer already registered");
        }

        [Fact]
        public async void Handle_WhenCommandIsInvalid_ReturnsFail()
        {
            // Arrange
            var handler = new CreateCustomerCommandHandler(
                _validator,
                Substitute.For<ICustomerWriteOnlyRepository>(),
                Substitute.For<IUnitOfWork>());

            // Act
            var res = await handler.Handle(new CreateCustomerCommand(), CancellationToken.None);

            // Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeFalse();
            res.ValidationErrors.Should().NotBeNullOrEmpty().And.OnlyHaveUniqueItems();
        }
    }
}

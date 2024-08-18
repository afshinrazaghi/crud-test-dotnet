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
    public class UpdateCustomerCommandHandlerTests : IClassFixture<EfSQLiteFixture>
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
            Presentation.Domain.Entities.CustomerAggregate.Customer customer = new Faker<Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate.Customer>().
                CustomInstantiator(faker => CustomerFactory.Create(
                    faker.Person.FirstName,
                    faker.Person.LastName,
                    faker.Person.DateOfBirth,
                    PhoneNumberFixture.Generate(),
                    faker.Person.Email,
                    BankAccountNumberFixture.Generate()
                )).Generate();

            CustomerWriteOnlyRepository repository = new CustomerWriteOnlyRepository(_fixture.Context);
            repository.Add(customer);

            await _fixture.Context.SaveChangesAsync();
            _fixture.Context.ChangeTracker.Clear();

            UnitOfWork unitOfWork = new UnitOfWork(
                _fixture.Context,
                Substitute.For<IEventStoreRepository>(),
                Substitute.For<IMediator>(),
                Substitute.For<ILogger<UnitOfWork>>()
            );

            UpdateCustomerCommand command = new Faker<UpdateCustomerCommand>()
                .RuleFor(command => command.OriginalEmail, customer.Email.Value)
                .RuleFor(command => command.FirstName, faker => faker.Person.FirstName)
                .RuleFor(command => command.LastName, faker => faker.Person.LastName)
                .RuleFor(command => command.DateOfBirth, faker => faker.Person.DateOfBirth)
                .RuleFor(command => command.PhoneNumber, faker => PhoneNumberFixture.Generate())
                .RuleFor(command => command.Email, faker => faker.Person.Email)
                .RuleFor(command => command.BankAccountNumber, faker => BankAccountNumberFixture.Generate())
                .Generate();

            UpdateCustomerCommandHandler handler = new UpdateCustomerCommandHandler(_validator, repository, unitOfWork);

            // Act
            Ardalis.Result.Result res = await handler.Handle(command, CancellationToken.None);

            // Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeTrue();
            res.SuccessMessage.Should().Be("Updated successfully!");
        }

        [Fact]
        public async void Handle_WhenDuplicateEmail_ReturnsFail()
        {
            // Arrange
            List<Presentation.Domain.Entities.CustomerAggregate.Customer> customers = new Faker<Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate.Customer>().
               CustomInstantiator(faker => CustomerFactory.Create(
                   faker.Person.FirstName,
                   faker.Person.LastName,
                   faker.Person.DateOfBirth,
                   PhoneNumberFixture.Generate(),
                   faker.Person.Email,
                   BankAccountNumberFixture.Generate()
               )).Generate(2);


            CustomerWriteOnlyRepository repository = new CustomerWriteOnlyRepository(_fixture.Context);
            foreach (Presentation.Domain.Entities.CustomerAggregate.Customer customer in customers)
            {
                repository.Add(customer);
            }

            await _fixture.Context.SaveChangesAsync();
            _fixture.Context.ChangeTracker.Clear();

            UpdateCustomerCommand command = new Faker<UpdateCustomerCommand>()
                .RuleFor(command => command.OriginalEmail, customers[0].Email.Value)
                .RuleFor(command => command.Email, _ => customers[1].Email.Value)
                .RuleFor(command => command.FirstName, faker => faker.Person.FirstName)
                .RuleFor(command => command.LastName, faker => faker.Person.LastName)
                .RuleFor(command => command.DateOfBirth, faker => faker.Person.DateOfBirth)
                .RuleFor(command => command.PhoneNumber, faker => PhoneNumberFixture.Generate())
                .RuleFor(command => command.BankAccountNumber, faker => BankAccountNumberFixture.Generate())
                .Generate();

            UpdateCustomerCommandHandler handler = new UpdateCustomerCommandHandler(
                _validator,
                repository,
                Substitute.For<IUnitOfWork>());

            // Act

            Ardalis.Result.Result res = await handler.Handle(command, CancellationToken.None);

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
            List<Presentation.Domain.Entities.CustomerAggregate.Customer> customers = new Faker<Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate.Customer>().
               CustomInstantiator(faker => CustomerFactory.Create(
                   faker.Person.FirstName,
                   faker.Person.LastName,
                   faker.Person.DateOfBirth,
                   PhoneNumberFixture.Generate(),
                   faker.Person.Email,
                   BankAccountNumberFixture.Generate()
               )).Generate(2);

            CustomerWriteOnlyRepository repository = new CustomerWriteOnlyRepository(_fixture.Context);
            foreach (Presentation.Domain.Entities.CustomerAggregate.Customer customer in customers)
            {
                repository.Add(customer);
            }

            await _fixture.Context.SaveChangesAsync();
            _fixture.Context.ChangeTracker.Clear();

            UpdateCustomerCommand command = new Faker<UpdateCustomerCommand>()
                .RuleFor(command => command.OriginalEmail, customers[0].Email.Value)
                .RuleFor(command => command.Email, customers[0].Email.Value)
                .RuleFor(command => command.FirstName, customers[1].FirstName)
                .RuleFor(command => command.LastName, customers[1].LastName)
                .RuleFor(command => command.DateOfBirth, customers[1].DateOfBirth)
                .RuleFor(command => command.PhoneNumber, faker => PhoneNumberFixture.Generate())
                .RuleFor(command => command.BankAccountNumber, faker => BankAccountNumberFixture.Generate())
                .Generate();


            UpdateCustomerCommandHandler handler = new UpdateCustomerCommandHandler(
                _validator,
                repository,
                Substitute.For<IUnitOfWork>());

            // Act
            Ardalis.Result.Result res = await handler.Handle(command, CancellationToken.None);

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
            UpdateCustomerCommand command = new Faker<UpdateCustomerCommand>()
                .RuleFor(command => command.OriginalEmail, faker => faker.Person.Email)
                .RuleFor(command => command.FirstName, faker => faker.Person.FirstName)
                .RuleFor(command => command.LastName, faker => faker.Person.LastName)
                .RuleFor(command => command.DateOfBirth, faker => faker.Person.DateOfBirth)
                .RuleFor(command => command.Email, faker => faker.Person.Email)
                .RuleFor(command => command.PhoneNumber, faker => PhoneNumberFixture.Generate())
                .RuleFor(command => command.BankAccountNumber, faker => BankAccountNumberFixture.Generate())
                .Generate();

            CustomerWriteOnlyRepository repository = new CustomerWriteOnlyRepository(_fixture.Context);

            UpdateCustomerCommandHandler handler = new UpdateCustomerCommandHandler(
                _validator,
                repository,
                Substitute.For<IUnitOfWork>()
            );

            // Act

            Ardalis.Result.Result res = await handler.Handle(command, CancellationToken.None);

            // Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeFalse();
            res.Errors.Should()
                .NotBeNullOrEmpty()
                .And.OnlyHaveUniqueItems()
                .And.Contain(errorMessage => errorMessage == $"No customer found by Email : {command.Email}");
        }


        [Fact]
        public async void Handle_WhenCommandInValid_ReturnsFail()
        {
            // Arrange
            UpdateCustomerCommandHandler handler = new UpdateCustomerCommandHandler(
                _validator,
                Substitute.For<ICustomerWriteOnlyRepository>(),
                Substitute.For<IUnitOfWork>());
            // Act

            Ardalis.Result.Result res = await handler.Handle(new UpdateCustomerCommand(), CancellationToken.None);

            // Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeFalse();
            res.ValidationErrors.Should().NotBeNullOrEmpty().And.OnlyHaveUniqueItems();
        }


    }
}

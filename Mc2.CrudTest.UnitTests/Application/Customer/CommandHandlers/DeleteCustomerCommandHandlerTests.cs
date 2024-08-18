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
    public class DeleteCustomerCommandHandlerTests : IClassFixture<EfSQLiteFixture>
    {
        private readonly DeleteCustomerCommandValidator validator = new DeleteCustomerCommandValidator();
        private readonly EfSQLiteFixture _fixture;

        public DeleteCustomerCommandHandlerTests(EfSQLiteFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async void Handle_WhenCalledWithValidCustomer_ReturnsSuccess()
        {
            // Arrange
            Presentation.Domain.Entities.CustomerAggregate.Customer customer = new Faker<Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate.Customer>()
                 .CustomInstantiator(faker => CustomerFactory.Create(
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
                    Substitute.For<ILogger<UnitOfWork>>());

            DeleteCustomerCommandHandler handler = new DeleteCustomerCommandHandler(
                validator,
                repository,
                unitOfWork);

            DeleteCustomerCommand command = new DeleteCustomerCommand() { Email = customer.Email.Value };

            // Act

            Ardalis.Result.Result res = await handler.Handle(command, CancellationToken.None);

            // Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeTrue();
            res.SuccessMessage.Should().Be("Successfully removed!");
        }

        [Fact]
        public async void Handle_WhenNotFound_ReturnsFail()
        {
            // Arrange
            CustomerWriteOnlyRepository repository = new CustomerWriteOnlyRepository(_fixture.Context);
            DeleteCustomerCommand command = new DeleteCustomerCommand("test@gmail.com");
            DeleteCustomerCommandHandler handler = new DeleteCustomerCommandHandler(
                validator,
                repository,
                Substitute.For<IUnitOfWork>()
            );


            // Act
            Ardalis.Result.Result res = await handler.Handle(command, CancellationToken.None);

            // Assert

            res.Should().NotBeNull();
            res.IsSuccess.Should().BeFalse();
            res.Errors
                .Should().NotBeNullOrEmpty()
                .And.OnlyHaveUniqueItems()
                .And.Contain(errorMessage => errorMessage == $"No customer found by Email : {command.Email}");
        }

        [Fact]
        public async void Handle_WhenInvalidCommand_ReturnsFail()
        {
            // Arrange

            DeleteCustomerCommandHandler handler = new DeleteCustomerCommandHandler(
                validator,
                Substitute.For<ICustomerWriteOnlyRepository>(),
                Substitute.For<IUnitOfWork>()
            );

            // Act
            Ardalis.Result.Result res = await handler.Handle(new DeleteCustomerCommand(), CancellationToken.None);

            // Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeFalse();
            res.ValidationErrors
                .Should().NotBeNullOrEmpty()
                .And.OnlyHaveUniqueItems();
        }
    }
}

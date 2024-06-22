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
    public class DeleteCustomerCommandHandlerTests
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
            var customer = new Faker<Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate.Customer>()
                 .CustomInstantiator(faker => CustomerFactory.Create(
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
                    Substitute.For<ILogger<UnitOfWork>>());

            var handler = new DeleteCustomerCommandHandler(
                validator,
                repository,
                unitOfWork);

            var command = new DeleteCustomerCommand() { Id = customer.Id };

            // Act

            var res = await handler.Handle(command, CancellationToken.None);

            // Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeTrue();
            res.SuccessMessage.Should().Be("Successfully removed!");
        }

        [Fact]
        public async void Handle_WhenNotFound_ReturnsFail()
        {
            // Arrange
            var repository = new CustomerWriteOnlyRepository(_fixture.Context);
            var command = new DeleteCustomerCommand(Guid.NewGuid());
            var handler = new DeleteCustomerCommandHandler(
                validator,
                repository,
                Substitute.For<IUnitOfWork>()
            );


            // Act
            var res = await handler.Handle(command, CancellationToken.None);

            // Assert

            res.Should().NotBeNull();
            res.IsSuccess.Should().BeFalse();
            res.Errors
                .Should().NotBeNullOrEmpty()
                .And.OnlyHaveUniqueItems()
                .And.Contain(errorMessage => errorMessage == $"No customer found by Id : {command.Id}");
        }

        [Fact]
        public async void Handle_WhenInvalidCommand_ReturnsFail()
        {
            // Arrange

            var handler = new DeleteCustomerCommandHandler(
                validator,
                Substitute.For<ICustomerWriteOnlyRepository>(),
                Substitute.For<IUnitOfWork>()
            );

            // Act
            var res = await handler.Handle(new DeleteCustomerCommand(), CancellationToken.None);

            // Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeFalse();
            res.Errors
                .Should().NotBeNullOrEmpty()
                .And.OnlyHaveUniqueItems();
        }
    }
}

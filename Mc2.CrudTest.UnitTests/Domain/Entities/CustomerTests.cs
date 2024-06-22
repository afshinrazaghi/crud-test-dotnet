using Bogus;
using FluentAssertions;
using Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate;
using Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate.Events;
using Mc2.CrudTest.Presentation.Domain.Factories;
using Mc2.CrudTest.UnitTests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.UnitTests.Domain.Entities
{
    public class CustomerTests
    {
        [Fact]
        public void Create_WhenCalled_CreateCustomerCreatedEvent()
        {
            // Arrange

            var customerFaker = new Faker<Customer>("nl")
                .CustomInstantiator(faker => CustomerFactory.Create(
                    faker.Person.FirstName,
                    faker.Person.LastName,
                    faker.Person.DateOfBirth,
                    faker.Random.Number(100000000, 999999999).ToString(),
                    faker.Person.Email,
                    BankAccountNumberFixture.Generate()
                 ));

            // Act

            var res = customerFaker.Generate();

            // Assert

            res.DomainEvents.Should()
                .NotBeNullOrEmpty()
                .And.OnlyHaveUniqueItems()
                .And.ContainItemsAssignableTo<CustomerCreatedEvent>();
        }
    }
}

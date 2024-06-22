using FluentAssertions;
using Mc2.CrudTest.Presentation.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.UnitTests.Domain.ValuesObjects
{

    public class BankAccountNumberTests
    {
        [Theory]
        [InlineData("12 34 56 789")]
        [InlineData("44 53 24 567")]
        public void CreateBankAccountNumber_WhenIsValid_ReturnSuccess(string bankAccountNumber)
        {
            // Arrange

            // Act
            var res = BankAccountNumber.Create(bankAccountNumber);
            // Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().NotBeNull().And.BeOfType<BankAccountNumber>();
            res.Value.Value.Should().NotBeNullOrEmpty().And.Be(bankAccountNumber);
        }

        [Theory]
        [InlineData("A-B-C")]
        [InlineData("22334")]
        [InlineData("443434")]
        [InlineData("123456789")]
        public void CreateBankAccountNumber_WhenInValid_ReturnFail(string bankAccoutNumber)
        {
            // Arrange

            // Act
            var res = BankAccountNumber.Create(bankAccoutNumber);
            // Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeFalse();
            res.Value.Should().BeNull();
            res.Errors.Should().NotBeNullOrEmpty()
                .And.OnlyHaveUniqueItems()
                .And.HaveCount(1)
                .And.Satisfy(errorMessage => errorMessage == "Back Account Number is not valid");

        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void CreateBankAccountNumber_WhenIsEmpty_ReturnFail(string bankAccountNumber)
        {
            // Arrange

            // Act
            var res = BankAccountNumber.Create(bankAccountNumber);

            // Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeFalse();
            res.Value.Should().BeNull();
            res.Errors.Should().NotBeNullOrEmpty()
                .And.OnlyHaveUniqueItems()
                .And.HaveCount(1)
                .And.Satisfy(errorMessage => errorMessage == "Back Account Number cannot be empty");
        }
    }
}

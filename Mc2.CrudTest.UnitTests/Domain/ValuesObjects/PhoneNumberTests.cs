using FluentAssertions;
using Mc2.CrudTest.Presentation.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.UnitTests.Domain.ValuesObjects
{
    public class PhoneNumberTests
    {
        [Theory]
        [InlineData("243611111")]
        [InlineData("201234567")]
        [InlineData("20 123 4567")]
        [InlineData("6 12345678")]
        [InlineData("10 123 4567")]
        [InlineData("70 123 4567")]
        [InlineData("30 123 4567")]
        public void CreatePhoneNumber_WhenPhoneNumberIsValid_ReturnsSuccess(string phoneNumber)
        {
            // Arrange

            // Act
            var res = PhoneNumber.Create(phoneNumber);

            // Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().NotBeNull().And.BeOfType<PhoneNumber>();
            res.Value.Value.Should().NotBeNullOrEmpty().And.Be(phoneNumber.Replace(" ", ""));
        }


        [Theory]
        [InlineData("0123")]
        [InlineData("12345678")]
        [InlineData("203344534567")]
        [InlineData("ABC")]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(" - ")]
        public void CreatePhoneNumber_WhenPhoneNumberNotValid_ReturnsFail(string phoneNumber)
        {
            // Arrange

            // Act
            var res = PhoneNumber.Create(phoneNumber);

            // Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeFalse();
            res.Value.Should().BeNull();
            res.Errors.Should().NotBeNullOrEmpty()
                .And.OnlyHaveUniqueItems()
                .And.HaveCount(1)
                .And.Satisfy(message => message == "Mobile Number is not valid");

        }
        
    }
}

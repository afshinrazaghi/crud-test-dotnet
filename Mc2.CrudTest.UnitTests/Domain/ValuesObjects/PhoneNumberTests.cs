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
        [InlineData("+989195512635")]
        [InlineData("+989123377891")]
        public void CreatePhoneNumber_WhenPhoneNumberIsValid_ReturnsSuccess(string phoneNumber)
        {
            // Arrange

            // Act
            Ardalis.Result.Result<PhoneNumber> res = PhoneNumber.Create(phoneNumber);

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
            Ardalis.Result.Result<PhoneNumber> res = PhoneNumber.Create(phoneNumber);

            // Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeFalse();
            res.Value.Should().BeNull();
            res.ValidationErrors.Should().NotBeNullOrEmpty()
                .And.OnlyHaveUniqueItems()
                .And.HaveCount(1)
                .And.Satisfy(message => message.ErrorMessage == "Mobile Number is not valid");

        }
        
    }
}

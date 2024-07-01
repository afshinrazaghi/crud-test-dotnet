using FluentAssertions;
using Mc2.CrudTest.Presentation.Domain.ValueObjects;
using Mc2.CrudTest.Presentation.Domain.ValueObjects.Validators;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.UnitTests.Custom
{
    public class PhoneAndMobileNumberTests
    {
        [Theory]
        [InlineData("+16156381234")]
        [InlineData("+989121234567")]
        public void CheckPhoneNumber_WhenCalledWithMobile_ReturnsTrue(string mobileNumber)
        {
            // Arrange
            var phoneNumberUtil = PhoneNumberUtil.GetInstance();
            var parsedPhoneNumber = phoneNumberUtil.Parse(mobileNumber, null);
            // Act
            var res = phoneNumberUtil.GetNumberType(parsedPhoneNumber);

            // Assert
            res.Should().BeOneOf(PhoneNumberType.FIXED_LINE_OR_MOBILE, PhoneNumberType.MOBILE);
                
        }


        [Fact]
        public void CheckPhoneNumber_WhenCalledWithPhoneNumber_ReturnsFalse()
        {
            // Arrange
            string myNumber = "+982188776655";
            var phoneNumberUtil = PhoneNumberUtil.GetInstance();
            var parsedPhoneNumber = phoneNumberUtil.Parse(myNumber, "IR");
            // Act
            var res = phoneNumberUtil.GetNumberType(parsedPhoneNumber);

            // Assert
            res.Should().NotBe(PhoneNumberType.MOBILE);
        }
    }
}

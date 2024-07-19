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
            PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();
            PhoneNumbers.PhoneNumber parsedPhoneNumber = phoneNumberUtil.Parse(mobileNumber, null);
            // Act
            PhoneNumberType res = phoneNumberUtil.GetNumberType(parsedPhoneNumber);

            // Assert
            res.Should().BeOneOf(PhoneNumberType.FIXED_LINE_OR_MOBILE, PhoneNumberType.MOBILE);
                
        }


        [Fact]
        public void CheckPhoneNumber_WhenCalledWithPhoneNumber_ReturnsFalse()
        {
            // Arrange
            string myNumber = "+982188776655";
            PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();
            PhoneNumbers.PhoneNumber parsedPhoneNumber = phoneNumberUtil.Parse(myNumber, "IR");
            // Act
            PhoneNumberType res = phoneNumberUtil.GetNumberType(parsedPhoneNumber);

            // Assert
            res.Should().NotBe(PhoneNumberType.MOBILE);
        }
    }
}

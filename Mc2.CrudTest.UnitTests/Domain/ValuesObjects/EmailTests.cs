using FluentAssertions;
using Mc2.CrudTest.Presentation.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.UnitTests.Domain.ValuesObjects
{
    public class EmailTests
    {
        [Theory]
        [InlineData("afshin.razaghi.net@gmail.com")]
        [InlineData("a@bcd.com")]
        [InlineData("123@ggg.com")]
        [InlineData("a-bc@ggg.edu.com")]
        public void CreateEmail_WhenEmailIsValid_ReturnsSuccess(string email)
        {
            // Arrange

            // Act
            Ardalis.Result.Result<Email> res = Email.Create(email);
            // Assert

            res.Should().NotBeNull();
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().NotBeNull().And.BeOfType<Email>();
            res.Value.Value.Should().NotBeNullOrEmpty().And.Be(email.ToLowerInvariant());
        }

        [Theory]
        [InlineData("a.b.c")]
        [InlineData("c")]
        [InlineData("q.bcc")]
        [InlineData("a@")]
        [InlineData("@google.com")]
        public void CreateEmail_WhenEmailInValid_ReturnsFail(string email)
        {
            // Arrange

            // Act
            Ardalis.Result.Result<Email> res = Email.Create(email);
            // Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeFalse();
            res.Value.Should().BeNull();
            res.ValidationErrors.Should().NotBeNullOrEmpty()
                .And.OnlyHaveUniqueItems()
                .And.HaveCount(1)
                .And.Satisfy(errorMessage => errorMessage.ErrorMessage == "Email is not valid");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void CreateEmail_WhenEmailIsNotOrEmpty_ReturnFail(string email)
        {
            // Arrange

            // Act
            Ardalis.Result.Result<Email> res = Email.Create(email);
            // Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeFalse();
            res.Value.Should().BeNull();
            res.ValidationErrors.Should().NotBeNullOrEmpty()
                .And.OnlyHaveUniqueItems()
                .And.HaveCount(1)
                .And.Satisfy(errorMessage => errorMessage.ErrorMessage == "Email cannot be empty");
        }


    }
}

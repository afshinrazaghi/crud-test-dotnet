using FluentAssertions;
using Mc2.CrudTest.Presentation.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.UnitTests.Shared.Extensions
{
    public class JsonExtensionsTests
    {
        private const string UserJson =
            "{\"email\":\"afshin.razaghi.net@gmail.com\",\"userName\":\"afshin.razaghi\",\"status\":\"active\"}";


        [Fact]
        public void FromJson_WhenCalled_ReturnSerialized()
        {
            // Arrange
            User user = new User("afshin.razaghi", "afshin.razaghi.net@gmail.com", EStatus.Active);

            // Act
            string res = user.ToJson();

            // Assert
            res.Should().NotBeNullOrEmpty().And.BeEquivalentTo(UserJson);
        }

        [Fact]
        public void FromJson_WhenCalled_ReturnDeserializeTyped()
        {
            // Arrange
            User expectedUser = new User("afshin.razaghi", "afshin.razaghi.net@gmail.com", EStatus.Active);
            // Act
            User res = UserJson.FromJson<User>();
            // Assert
            res.Should().NotBeNull().And.BeEquivalentTo(expectedUser);
            res.UserName.Should().NotBeNullOrWhiteSpace();
            res.Email.Should().NotBeNullOrWhiteSpace();
            res.Status.Should().Be(EStatus.Active);
        }

        [Fact]
        public void ToJson_WhenCalledWithNull_ReturnsNull()
        {
            // Arrange
            User? user = null;

            // Act
            string res = user.ToJson();

            // Assert
            res.Should().BeNull();
        }

        [Fact]
        public void FromJson_WhenCalledWithNull_ReturnsNull()
        {
            // Arrange
            const string? userJson = null;

            // Act
            User res = userJson.FromJson<User>();

            // Assert
            res.Should().BeNull();

        }


        private enum EStatus
        {
            Active = 0,
            Inactive = 1
        }


        private record User(string UserName, string Email, EStatus Status)
        {
            public string Email { get; } = Email;
            public string UserName { get; } = UserName;
            public EStatus Status { get; } = Status;
        }
    }
}

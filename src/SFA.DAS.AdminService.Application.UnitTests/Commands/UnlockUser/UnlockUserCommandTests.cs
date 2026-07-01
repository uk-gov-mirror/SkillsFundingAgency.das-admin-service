using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AdminService.Application.Commands.UnlockUser;

namespace SFA.DAS.AdminService.Application.UnitTests.Commands.UnlockUser
{
    [TestFixture]
    public class UnlockUserCommandTests
    {
        [Test]
        public void Command_ShouldSetProperties()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var command = new UnlockUserCommand
            {
                UserId = userId,
                Username = "admin",
                UserActionId = 123
            };

            // Assert
            command.UserId.Should().Be(userId);
            command.Username.Should().Be("admin");
            command.UserActionId.Should().Be(123);
        }
    }
}

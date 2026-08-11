using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AdminService.Application.Commands.CheckUserActionByCode;
using SFA.DAS.AdminService.Infrastructure.Api.Requests;

namespace SFA.DAS.AdminService.Application.UnitTests.Commands.CheckUserActionByCode
{
    [TestFixture]
    public class CheckUserActionByCodeCommandTests
    {
        [Test]
        public void Should_Convert_To_CheckUserActionByCodeRequest_Successfully()
        {
            // Arrange
            var command = new CheckUserActionByCodeCommand
            {
                Code = "code-1",
                Username = "user@example.com"
            };

            // Act
            CheckUserActionByCodeRequest request = command;

            // Assert
            request.Should().NotBeNull();
            request.Username.Should().Be(command.Username);
        }
    }
}

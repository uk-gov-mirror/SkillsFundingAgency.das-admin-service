using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Application.Commands.UnlockUser;
using SFA.DAS.AdminService.Infrastructure.Api.Requests;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Admin;

namespace SFA.DAS.AdminService.Application.UnitTests.Commands.UnlockUser
{
    [TestFixture]
    public class UnlockUserCommandHandlerTests
    {
        private Mock<IAdminOuterApi> _adminOuterApiMock;
        private Mock<ILogger<UnlockUserCommandHandler>> _loggerMock;
        private UnlockUserCommandHandler _sut;

        [SetUp]
        public void Setup()
        {
            _adminOuterApiMock = new Mock<IAdminOuterApi>();
            _loggerMock = new Mock<ILogger<UnlockUserCommandHandler>>();
            _sut = new UnlockUserCommandHandler(_adminOuterApiMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task Handle_CallsAdminOuterApiUnlockUser_WithConvertedRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cmd = new UnlockUserCommand { UserId = userId, Username = "admin", UserActionId = 123 };

            // Act
            await _sut.Handle(cmd, CancellationToken.None);

            // Assert
            _adminOuterApiMock.Verify(a => a.UnlockUser(userId, It.Is<UnlockUserRequest>(r => r.Username == "admin" && r.UserActionId == 123)), Times.Once);
        }
    }
}

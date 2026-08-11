using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Application.Commands.CheckUserActionByCode;
using SFA.DAS.AdminService.Infrastructure.Api.Requests;
using SFA.DAS.AdminService.Infrastructure.Api.Responses;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Admin;

namespace SFA.DAS.AdminService.Application.UnitTests.Commands.CheckUserActionByCode
{
    [TestFixture]
    public class CheckUserActionByCodeCommandHandlerTests
    {
        private Mock<IAdminOuterApi> _adminApiMock;
        private Mock<ILogger<CheckUserActionByCodeCommandHandler>> _loggerMock;
        private CheckUserActionByCodeCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _adminApiMock = new Mock<IAdminOuterApi>();
            _loggerMock = new Mock<ILogger<CheckUserActionByCodeCommandHandler>>();
            _handler = new CheckUserActionByCodeCommandHandler(_adminApiMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task Handle_ReturnsMappedResult_WhenApiReturnsResponse()
        {
            var response = new CheckUserActionByCodeResponse
            {
                Id = 1,
                UserId = Guid.NewGuid(),
                ActionType = "Reprint",
                ActionTime = DateTime.UtcNow,
                ActionStatus = "New",
                Uln = 123,
                FamilyName = "Smith",
                GivenNames = "John",
                CertificateId = Guid.NewGuid(),
                CertificateType = "Standard",
                CourseName = "Course",
                AdminActions = new List<AdminActionResponse> { new AdminActionResponse { Username = "admin", ActionTime = DateTime.UtcNow, Action = "Viewed" } }
            };

            _adminApiMock.Setup(x => x.CheckUserActionByCode("code123", It.IsAny<CheckUserActionByCodeRequest>()))
                .ReturnsAsync(response);

            var result = await _handler.Handle(new CheckUserActionByCodeCommand { Code = "code123", Username = "user1" }, CancellationToken.None);

            result.Should().NotBeNull();
            result.Id.Should().Be(response.Id);
            result.UserId.Should().Be(response.UserId);
            result.ActionType.Should().Be(Common.Models.ActionType.Reprint);
            result.ActionStatus.Should().Be(Common.Models.UserActionStatus.New);
            result.FamilyName.Should().Be(response.FamilyName);
            _adminApiMock.Verify(x => x.CheckUserActionByCode("code123", It.IsAny<CheckUserActionByCodeRequest>()), Times.Once);
        }

        [Test]
        public async Task Handle_Calls_OuterApi_With_Correct_Request()
        {
            // Arrange
            var command = new CheckUserActionByCodeCommand { Code = "code123", Username = "user1" };
            var response = new CheckUserActionByCodeResponse
            {
                Id = 2,
                UserId = Guid.NewGuid(),
                ActionType = "Reprint",
                ActionTime = DateTime.UtcNow,
                ActionStatus = "New"
            };

            _adminApiMock.Setup(x => x.CheckUserActionByCode(It.IsAny<string>(), It.IsAny<CheckUserActionByCodeRequest>()))
                .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _adminApiMock.Verify(x => x.CheckUserActionByCode("code123", It.Is<CheckUserActionByCodeRequest>(r => r.Username == "user1")), Times.Once);
        }

        [Test]
        public void Handle_Throws_If_OuterApi_Fails()
        {
            // Arrange
            _adminApiMock.Setup(x => x.CheckUserActionByCode(It.IsAny<string>(), It.IsAny<CheckUserActionByCodeRequest>()))
                .ThrowsAsync(new Exception("API failure"));

            // Act
            Func<Task> act = async () => await _handler.Handle(new CheckUserActionByCodeCommand { Code = "c", Username = "u" }, CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<Exception>().WithMessage("API failure");
            _adminApiMock.Verify(x => x.CheckUserActionByCode(It.IsAny<string>(), It.IsAny<CheckUserActionByCodeRequest>()), Times.Once);
        }
    }
}

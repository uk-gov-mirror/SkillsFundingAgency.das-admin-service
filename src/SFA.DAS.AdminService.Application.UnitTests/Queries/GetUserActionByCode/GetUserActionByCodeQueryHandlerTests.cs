using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Application.Queries.GetUserActionByCode;
using SFA.DAS.AdminService.Infrastructure.Api.Responses;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Admin;

namespace SFA.DAS.AdminService.Application.UnitTests.Queries.GetUserActionByCode
{
    [TestFixture]
    public class GetUserActionByCodeQueryHandlerTests
    {
        private Mock<IAdminOuterApi> _adminApiMock;
        private Mock<ILogger<GetUserActionByCodeQueryHandler>> _loggerMock;
        private GetUserActionByCodeQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _adminApiMock = new Mock<IAdminOuterApi>();
            _loggerMock = new Mock<ILogger<GetUserActionByCodeQueryHandler>>();
            _handler = new GetUserActionByCodeQueryHandler(_adminApiMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task Handle_Returns_Mapped_Result_When_Api_Returns_Response()
        {
            // Arrange
            var response = new GetUserActionByCodeResponse
            {
                Id = 1,
                UserId = Guid.NewGuid(),
                ActionType = "NotMatched",
                ActionTime = DateTime.UtcNow,
                ActionStatus = "New",
                Uln = 1234567890,
                FamilyName = "Doe",
                GivenNames = "Jane",
                CertificateId = Guid.NewGuid(),
                CertificateType = "Standard",
                CourseName = "Course 1"
            };

            _adminApiMock.Setup(x => x.GetUserActionByCode("code123")).ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(new GetUserActionByCodeQuery { Code = "code123" }, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(response.Id);
            result.UserId.Should().Be(response.UserId);
            result.ActionType.ToString().Should().Be(response.ActionType);
            result.ActionStatus.ToString().Should().Be(response.ActionStatus);
            result.Uln.Should().Be(response.Uln);
            result.FamilyName.Should().Be(response.FamilyName);
            result.GivenNames.Should().Be(response.GivenNames);
            result.CertificateId.Should().Be(response.CertificateId);
            result.CertificateType.ToString().Should().Be(response.CertificateType);
            result.CourseName.Should().Be(response.CourseName);

            _adminApiMock.Verify(x => x.GetUserActionByCode("code123"), Times.AtLeastOnce);
        }

        [Test]
        public async Task Handle_Calls_OuterApi_With_Correct_Code()
        {
            // Arrange
            _adminApiMock.Setup(x => x.GetUserActionByCode(It.IsAny<string>())).ReturnsAsync(new GetUserActionByCodeResponse());

            var query = new GetUserActionByCodeQuery { Code = "abc123" };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _adminApiMock.Verify(x => x.GetUserActionByCode("abc123"), Times.Once);
        }

        [Test]
        public void Handle_Throws_If_OuterApi_Fails()
        {
            // Arrange
            _adminApiMock.Setup(x => x.GetUserActionByCode(It.IsAny<string>())).ThrowsAsync(new Exception("API failure"));

            // Act
            Func<Task> act = async () => await _handler.Handle(new GetUserActionByCodeQuery { Code = "c" }, CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<Exception>().WithMessage("API failure");
        }
    }
}

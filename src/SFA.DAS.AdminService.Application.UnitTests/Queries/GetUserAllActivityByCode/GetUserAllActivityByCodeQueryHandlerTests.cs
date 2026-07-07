using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Application.Queries.GetUserAllActivityByCode;
using SFA.DAS.AdminService.Infrastructure.Api.Responses;
using SFA.DAS.AdminService.Common.Models;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Admin;

namespace SFA.DAS.AdminService.Application.UnitTests.Queries.GetUserAllActivityByCode
{
    [TestFixture]
    public class GetUserAllActivityByCodeQueryHandlerTests
    {
        private Mock<IAdminOuterApi> _adminApiMock;
        private Mock<ILogger<GetUserAllActivityByCodeQueryHandler>> _loggerMock;
        private GetUserAllActivityByCodeQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _adminApiMock = new Mock<IAdminOuterApi>();
            _loggerMock = new Mock<ILogger<GetUserAllActivityByCodeQueryHandler>>();
            _handler = new GetUserAllActivityByCodeQueryHandler(_adminApiMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task Handle_ReturnsMappedResult_WhenApiReturnsResponse()
        {
            var response = new UserAllActivityResponse
            {
                UserId = Guid.NewGuid(),
                GovUKIdentifier = "GOV123",
                EmailAddress = "a@b.com",
                PhoneNumber = "012345",
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow.AddDays(-1),
                IsLocked = true,
                LockedTime = DateTime.UtcNow,
                UserActions = new List<UserActionResponse>
                {

                    new UserActionResponse { Id = 1, ActionCode = "REF1", ActionType = "NotMatched", ActionTime = DateTime.UtcNow, ActionStatus = UserActionStatus.New.ToString(), GivenNames = "Jane", FamilyName = "Doe", CertificateType = CertificateType.Standard.ToString() }
                }
            };

            _adminApiMock.Setup(x => x.GetUserAllActivity("code123")).ReturnsAsync(response);

            var result = await _handler.Handle(new GetUserAllActivityByCodeQuery { Code = "code123" }, CancellationToken.None);

            result.Should().NotBeNull();
            result.UserId.Should().Be(response.UserId);
            result.GovUKIdentifier.Should().Be(response.GovUKIdentifier);
            result.EmailAddress.Should().Be(response.EmailAddress);
            result.PhoneNumber.Should().Be(response.PhoneNumber);
            result.IsLocked.Should().BeTrue();

            result.UserActions.Should().NotBeNull();
            result.UserActions.Count.Should().Be(response.UserActions.Count);
            var expectedAction = response.UserActions[0];
            var actualAction = result.UserActions[0];
            actualAction.Id.Should().Be(expectedAction.Id);
            actualAction.ActionCode.Should().Be(expectedAction.ActionCode);
            actualAction.ActionType.Should().Be(ActionType.NotMatched);
            actualAction.ActionTime.Should().Be(expectedAction.ActionTime);
            actualAction.ActionStatus.Should().Be(UserActionStatus.New);
            actualAction.GivenNames.Should().Be(expectedAction.GivenNames);
            actualAction.FamilyName.Should().Be(expectedAction.FamilyName);
            actualAction.CertificateType.Should().Be(CertificateType.Standard);
            _adminApiMock.Verify(x => x.GetUserAllActivity("code123"), Times.AtLeastOnce);
        }

        [Test]
        public async Task Handle_Calls_OuterApi_With_Correct_Code()
        {
            _adminApiMock.Setup(x => x.GetUserAllActivity(It.IsAny<string>())).ReturnsAsync(new UserAllActivityResponse { GovUKIdentifier = "", EmailAddress = "", PhoneNumber = "" });

            var query = new GetUserAllActivityByCodeQuery { Code = "abc123" };

            var result = await _handler.Handle(query, CancellationToken.None);

            _adminApiMock.Verify(x => x.GetUserAllActivity("abc123"), Times.Once);
        }

        [Test]
        public void Handle_Throws_If_OuterApi_Fails()
        {
            _adminApiMock.Setup(x => x.GetUserAllActivity(It.IsAny<string>())).ThrowsAsync(new Exception("API failure"));

            Func<Task> act = async () => await _handler.Handle(new GetUserAllActivityByCodeQuery { Code = "c" }, CancellationToken.None);

            act.Should().ThrowAsync<Exception>().WithMessage("API failure");
        }
    }
}

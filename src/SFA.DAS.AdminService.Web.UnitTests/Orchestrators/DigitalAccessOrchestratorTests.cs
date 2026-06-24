using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Application.Commands.CheckUserActionByCode;
using SFA.DAS.AdminService.Application.Commands.GetUserAllActivityByCode;
using SFA.DAS.AdminService.Infrastructure.Api.Responses;
using SFA.DAS.AdminService.Application.Models;
using SFA.DAS.AdminService.Web.Orchestrators;
using SFA.DAS.AdminService.Common.Models;

namespace SFA.DAS.AdminService.Web.UnitTests.Orchestrators
{
    [TestFixture]
    public class DigitalAccessOrchestratorTests
    {
        private Mock<IMediator> _mediatorMock;
        private DigitalAccessOrchestrator _sut;

        [SetUp]
        public void SetUp()
        {
            _mediatorMock = new Mock<IMediator>();
            _sut = new DigitalAccessOrchestrator(_mediatorMock.Object);
        }

        [Test]
        public async Task FindUserActionByReference_ReturnsNull_WhenMediatorReturnsNull()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<CheckUserActionByCodeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CheckUserActionByCodeCommandResult)null);

            var vm = await _sut.GetDigitalAccessReferenceViewModel("ref1", "user1");

            vm.Should().BeNull();
            _mediatorMock.Verify(m => m.Send(It.IsAny<CheckUserActionByCodeCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task FindUserActionByReference_ReturnsMappedViewModel_WhenMediatorReturnsResult()
        {
            var response = new CheckUserActionByCodeCommandResult
            {
                Id = 1,
                UserId = Guid.NewGuid(),
                ActionType = ActionType.Reprint,
                ActionTime = DateTime.UtcNow,
                ActionStatus = UserActionStatus.Viewed,
                Uln = 123,
                FamilyName = "Smith",
                GivenNames = "John",
                CertificateId = Guid.NewGuid(),
                CertificateType = CertificateType.Standard,
                CourseName = "Course",
                AdminActions = new List<AdminAction>
                {
                    new AdminAction { Username = "admin", ActionTime = DateTime.UtcNow, Action = "Viewed" }
                }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CheckUserActionByCodeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var vm = await _sut.GetDigitalAccessReferenceViewModel("ref2", "user2");

            vm.Should().NotBeNull();
            vm.ReferenceNumber.Should().Be("ref2");
            vm.ActionType.Should().Be(response.ActionType);
            _mediatorMock.Verify(m => m.Send(It.IsAny<CheckUserActionByCodeCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetUserNotFoundViewModel_ReturnsNull_WhenMediatorReturnsNull()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<CheckUserActionByCodeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CheckUserActionByCodeCommandResult)null);

            var vm = await _sut.GetUserNotFoundViewModel("ref3", "user3");

            vm.Should().BeNull();
            _mediatorMock.Verify(m => m.Send(It.IsAny<CheckUserActionByCodeCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetUserNotFoundViewModel_ReturnsNames_WhenMediatorReturnsResult()
        {
            var response = new CheckUserActionByCodeCommandResult
            {
                GivenNames = "Diane",
                FamilyName = "Lockhart"
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CheckUserActionByCodeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var vm = await _sut.GetUserNotFoundViewModel("ref4", "user4");

            vm.Should().NotBeNull();
            vm.ReferenceNumber.Should().Be("ref4");
            vm.FirstName.Should().Be("Diane");
            vm.LastName.Should().Be("Lockhart");
            _mediatorMock.Verify(m => m.Send(It.IsAny<CheckUserActionByCodeCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetUserNotMatchedViewModel_ReturnsNull_WhenMediatorReturnsNull()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserAllActivityByCodeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetUserAllActivityByCodeCommandResult)null);

            var vm = await _sut.GetUserNotMatchedViewModel("ref-nm");

            vm.Should().BeNull();
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetUserAllActivityByCodeCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetUserNotMatchedViewModel_ReturnsMappedViewModel_WhenMediatorReturnsResult()
        {
            var response = new GetUserAllActivityByCodeCommandResult
            {
                IsLocked = true,
                GovUKIdentifier = "GOV123",
                EmailAddress = "alice@example.com",
                PhoneNumber = "0123456789",
                UserActions = new List<UserActionResponse>
                {
                    new UserActionResponse
                    {
                        Id = 1,
                        ActionCode = "REF1",
                        ActionType = "NotMatched",
                        ActionTime = DateTime.UtcNow,
                        ActionStatus = "Failed",
                        GivenNames = "Alice",
                        FamilyName = "Jones",
                        CertificateType = CertificateType.Standard.ToString(),
                        UserMatches = new List<UserMatchResponse>
                        {
                            new UserMatchResponse { EventTime = DateTime.UtcNow.AddMinutes(-5), Uln = 111, CourseName = "Course A", DateAwarded = 2020, ProviderName = "Provider A", FamilyName = "Jones", CertificateType = CertificateType.Standard.ToString() },
                            new UserMatchResponse { EventTime = DateTime.UtcNow, Uln = 222, CourseName = "Course B", DateAwarded = 2021, ProviderName = "Provider B", FamilyName = "Jones", CertificateType = CertificateType.Standard.ToString() }
                        },
                        AdminActions = new List<AdminActionResponse>
                        {
                            new AdminActionResponse { Username = "admin", Action = "Unlocked", ActionTime = DateTime.UtcNow.AddMinutes(1) }
                        }
                    }
                }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserAllActivityByCodeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var vm = await _sut.GetUserNotMatchedViewModel("ref-nm");

            vm.Should().NotBeNull();
            vm.ReferenceNumber.Should().Be("ref-nm");
            vm.FirstName.Should().Be("Alice");
            vm.LastName.Should().Be("Jones");
            vm.IsUserLocked.Should().BeTrue();
            vm.History.Should().HaveCount(1);
            var item = vm.History[0];
            item.TagText.Should().Be(Web.Constants.DigitalAccessConstants.TagTextUnlocked );
            item.Attempts.Should().HaveCount(2);
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetUserAllActivityByCodeCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Application.Commands.CheckUserActionByCode;
using SFA.DAS.AdminService.Infrastructure.Api.Responses;
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
        public async Task FindUserActionByReference_ReturnsReferenceOnly_WhenMediatorReturnsNull()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<CheckUserActionByCodeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CheckUserActionByCodeCommandResult)null);

            var vm = await _sut.GetDigitalAccessReferenceViewModel("ref1", "user1");

            vm.Should().NotBeNull();
            vm.ReferenceNumber.Should().Be("ref1");
            vm.Result.Should().BeNull();
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
                AdminActions = new List<AdminActionResponse>
                {
                    new AdminActionResponse { Username = "admin", ActionTime = DateTime.UtcNow, Action = "Viewed" }
                }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CheckUserActionByCodeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var vm = await _sut.GetDigitalAccessReferenceViewModel("ref2", "user2");

            vm.Should().NotBeNull();
            vm.ReferenceNumber.Should().Be("ref2");
            vm.Result.Should().NotBeNull();
            vm.Result.Id.Should().Be(response.Id);
            vm.Result.UserId.Should().Be(response.UserId);
            vm.Result.ActionType.Should().Be(response.ActionType);
            vm.Result.ActionStatus.Should().Be(response.ActionStatus);
            vm.Result.FamilyName.Should().Be(response.FamilyName);
            vm.Result.AdminActions.Should().NotBeNull();
            vm.Result.AdminActions.Count.Should().Be(1);
            vm.Result.AdminActions[0].Username.Should().Be("admin");
            vm.Result.AdminActions[0].Action.Should().Be(AdminActionType.Viewed);
        }

        [Test]
        public async Task GetUserNotFoundViewModel_ReturnsReferenceOnly_WhenMediatorReturnsNull()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<CheckUserActionByCodeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CheckUserActionByCodeCommandResult)null);

            var vm = await _sut.GetUserNotFoundViewModel("ref3", "user3");

            vm.Should().NotBeNull();
            vm.ReferenceNumber.Should().Be("ref3");
            vm.FirstName.Should().BeNullOrEmpty();
            vm.LastName.Should().BeNullOrEmpty();
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
        }
    }
}

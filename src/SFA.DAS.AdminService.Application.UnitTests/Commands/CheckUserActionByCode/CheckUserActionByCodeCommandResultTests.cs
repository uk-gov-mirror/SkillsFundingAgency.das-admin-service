using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AdminService.Application.Commands.CheckUserActionByCode;
using SFA.DAS.AdminService.Common.Models;
using SFA.DAS.AdminService.Infrastructure.Api.Responses;

namespace SFA.DAS.AdminService.Application.UnitTests.Commands.CheckUserActionByCode
{
    [TestFixture]
    public class CheckUserActionByCodeCommandResultTests
    {
        [Test]
        public void Should_Convert_From_Response_To_Result_Successfully()
        {
            // Arrange
            var response = new CheckUserActionByCodeResponse
            {
                Id = 10,
                UserId = Guid.NewGuid(),
                ActionType = "Reprint",
                ActionTime = DateTime.UtcNow,
                ActionStatus = "New",
                Uln = 999,
                FamilyName = "Doe",
                GivenNames = "Jane",
                CertificateId = Guid.NewGuid(),
                CertificateType = "Standard",
                CourseName = "Course1",
                AdminActions = new List<AdminActionResponse> { new AdminActionResponse { Username = "admin", ActionTime = DateTime.UtcNow, Action = "Viewed" } }
            };

            // Act
            CheckUserActionByCodeCommandResult result = response;

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(response.Id);
            result.UserId.Should().Be(response.UserId);
            result.ActionType.Should().Be(ActionType.Reprint);
            result.ActionStatus.Should().Be(UserActionStatus.New);
            result.CertificateType.Should().Be(CertificateType.Standard);
            result.FamilyName.Should().Be("Doe");
            result.GivenNames.Should().Be("Jane");
            result.AdminActions.Should().NotBeNull();
            result.AdminActions.Count.Should().Be(1);
        }
    }
}

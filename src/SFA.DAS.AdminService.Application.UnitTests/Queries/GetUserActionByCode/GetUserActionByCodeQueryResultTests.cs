using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AdminService.Application.Queries.GetUserActionByCode;
using SFA.DAS.AdminService.Infrastructure.Api.Responses;
using SFA.DAS.AdminService.Common.Models;

namespace SFA.DAS.AdminService.Application.UnitTests.Queries.GetUserActionByCode
{
    [TestFixture]
    public class GetUserActionByCodeQueryResultTests
    {
        [Test]
        public void ImplicitOperator_Maps_Response_To_Result()
        {
            // Arrange
            var response = new GetUserActionByCodeResponse
            {
                Id = 1,
                UserId = Guid.NewGuid(),
                ActionType = ActionType.NotMatched.ToString(),
                ActionTime = DateTime.UtcNow,
                ActionStatus = UserActionStatus.New.ToString(),
                Uln = 123456,
                FamilyName = "Doe",
                GivenNames = "Jane",
                CertificateId = Guid.NewGuid(),
                CertificateType = CertificateType.Standard.ToString(),
                CourseName = "Course 1",
                AdminActions = new List<AdminActionResponse>
                {
                    new AdminActionResponse { Username = "u1", ActionTime = DateTime.UtcNow, Action = "Viewed" }
                }
            };

            // Act
            var result = (GetUserActionByCodeQueryResult)response;

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(response.Id);
            result.UserId.Should().Be(response.UserId);
            result.ActionType.Should().Be(ActionType.NotMatched);
            result.ActionStatus.Should().Be(UserActionStatus.New);
            result.CertificateType.Should().Be(CertificateType.Standard);
            result.AdminActions.Should().NotBeNull();
            result.AdminActions.Count.Should().Be(1);
        }
    }
}

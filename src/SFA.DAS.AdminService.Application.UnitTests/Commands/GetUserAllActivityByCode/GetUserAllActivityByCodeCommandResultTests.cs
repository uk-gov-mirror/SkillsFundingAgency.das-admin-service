using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AdminService.Application.Commands.GetUserAllActivityByCode;
using SFA.DAS.AdminService.Infrastructure.Api.Responses;

namespace SFA.DAS.AdminService.Application.UnitTests.Commands.GetUserAllActivityByCode
{
    [TestFixture]
    public class GetUserAllActivityByCodeCommandResultTests
    {
        [Test]
        public void ImplicitConversion_MapsFields_FromApiResponse()
        {
            var api = new UserAllActivityResponse
            {
                UserId = Guid.NewGuid(),
                GovUKIdentifier = "GOV123",
                EmailAddress = "a@b.com",
                PhoneNumber = "012345",
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow.AddDays(-1),
                IsLocked = false,
                LockedTime = null,
                UserActions = new System.Collections.Generic.List<UserActionResponse>()
            };

            GetUserAllActivityByCodeCommandResult result = api;

            result.Should().NotBeNull();
            result.UserId.Should().Be(api.UserId);
            result.GovUKIdentifier.Should().Be(api.GovUKIdentifier);
            result.EmailAddress.Should().Be(api.EmailAddress);
            result.PhoneNumber.Should().Be(api.PhoneNumber);
            result.IsLocked.Should().Be(api.IsLocked);
        }
    }
}

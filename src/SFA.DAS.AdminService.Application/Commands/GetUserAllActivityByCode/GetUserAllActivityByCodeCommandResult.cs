using System;
using System.Collections.Generic;
using SFA.DAS.AdminService.Infrastructure.Api.Responses;

namespace SFA.DAS.AdminService.Application.Commands.GetUserAllActivityByCode
{
    public class GetUserAllActivityByCodeCommandResult
    {
        public Guid UserId { get; set; }
        public required string GovUKIdentifier { get; set; }
        public required string EmailAddress { get; set; }
        public required string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LockedTime { get; set; }
        public List<UserActionResponse> UserActions { get; set; }

        public static implicit operator GetUserAllActivityByCodeCommandResult(UserAllActivityResponse source)
        {
            if (source == null) return null;

            return new GetUserAllActivityByCodeCommandResult
            {
                UserId = source.UserId,
                GovUKIdentifier = source.GovUKIdentifier,
                EmailAddress = source.EmailAddress,
                PhoneNumber = source.PhoneNumber,
                CreatedAt = source.CreatedAt,
                LastLoginAt = source.LastLoginAt,
                IsLocked = source.IsLocked,
                LockedTime = source.LockedTime,
                UserActions = source.UserActions
            };
        }
    }
}

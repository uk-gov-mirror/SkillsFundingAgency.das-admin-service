using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.AdminService.Infrastructure.Api.Responses;
using SFA.DAS.AdminService.Application.Models;
using SFA.DAS.AdminService.Common.Models;

namespace SFA.DAS.AdminService.Application.Queries.GetUserAllActivityByCode
{
    public class GetUserAllActivityByCodeQueryResult
    {
        public Guid UserId { get; set; }
        public required string GovUKIdentifier { get; set; }
        public required string EmailAddress { get; set; }
        public required string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LockedTime { get; set; }
        public List<UserAction> UserActions { get; set; }

        public static implicit operator GetUserAllActivityByCodeQueryResult(UserAllActivityResponse source)
        {
            if (source == null) return null;

            return new GetUserAllActivityByCodeQueryResult
            {
                UserId = source.UserId,
                GovUKIdentifier = source.GovUKIdentifier,
                EmailAddress = source.EmailAddress,
                PhoneNumber = source.PhoneNumber,
                CreatedAt = source.CreatedAt,
                LastLoginAt = source.LastLoginAt,
                IsLocked = source.IsLocked,
                LockedTime = source.LockedTime,
                UserActions = source.UserActions?.Select(ua => new UserAction
                {
                    Id = ua.Id,
                    UserId = ua.UserId,
                    ActionCode = ua.ActionCode,
                    ActionType = Enum.Parse<ActionType>(ua.ActionType, true),
                    ActionTime = ua.ActionTime,
                    ActionStatus = Enum.Parse<UserActionStatus>(ua.ActionStatus, true),
                    Uln = ua.Uln,
                    FamilyName = ua.FamilyName,
                    GivenNames = ua.GivenNames,
                    CertificateId = ua.CertificateId,
                    CertificateType = Enum.Parse<CertificateType>(ua.CertificateType, true),
                    CourseName = ua.CourseName,
                    UserMatches = ua.UserMatches?.Select(um => new UserMatch
                    {
                        Id = um.Id,
                        Uln = um.Uln,
                        FamilyName = um.FamilyName,
                        DateOfBirth = um.DateOfBirth,
                        EventTime = um.EventTime,
                        CertificateType = Enum.Parse<CertificateType>(um.CertificateType, true),
                        CourseCode = um.CourseCode,
                        CourseName = um.CourseName,
                        CourseLevel = um.CourseLevel,
                        DateAwarded = um.DateAwarded,
                        ProviderName = um.ProviderName,
                        Ukprn = um.Ukprn,
                        IsMatched = um.IsMatched,
                        IsFailed = um.IsFailed
                    }).ToList(),
                    AdminActions = ua.AdminActions?.Select(a => new AdminAction
                    {
                        Username = a.Username,
                        ActionTime = a.ActionTime,
                        Action = a.Action
                    }).ToList()
                }).ToList()
            };
        }
    }
}

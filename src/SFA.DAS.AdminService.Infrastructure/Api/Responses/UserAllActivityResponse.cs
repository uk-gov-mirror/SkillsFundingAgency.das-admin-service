using System;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Infrastructure.Api.Responses
{
    public class UserAllActivityResponse
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
    }

    public class UserActionResponse
    {
        public long Id { get; set; }
        public Guid UserId { get; set; }
        public required string ActionCode { get; set; }
        public required string ActionType { get; set; }
        public DateTime ActionTime { get; set; }
        public required string ActionStatus { get; set; }
        public long? Uln { get; set; }
        public required string FamilyName { get; set; }
        public required string GivenNames { get; set; }
        public Guid? CertificateId { get; set; }
        public required string CertificateType { get; set; }
        public string CourseName { get; set; }
        public List<UserMatchResponse> UserMatches { get; set; }
        public List<AdminActionResponse> AdminActions { get; set; }
    }

    public class UserMatchResponse
    {
        public Guid Id { get; set; }
        public long? Uln { get; set; }
        public required string FamilyName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime EventTime { get; set; }
        public required string CertificateType { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string CourseLevel { get; set; }
        public int? DateAwarded { get; set; }
        public string ProviderName { get; set; }
        public int? Ukprn { get; set; }
        public bool IsMatched { get; set; }
        public bool IsFailed { get; set; }
    }
}

using System;
using SFA.DAS.AdminService.Common.Models;

namespace SFA.DAS.AdminService.Application.Models
{
    public class UserMatch
    {
        public Guid Id { get; set; }
        public long? Uln { get; set; }
        public required string FamilyName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime EventTime { get; set; }
        public CertificateType CertificateType { get; set; }
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

using System;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Application.Models
{
    using SFA.DAS.AdminService.Common.Models;

    public class UserAction
    {
        public long Id { get; set; }
        public Guid UserId { get; set; }
        public required string ActionCode { get; set; }
        public ActionType ActionType { get; set; }
        public DateTime ActionTime { get; set; }
        public UserActionStatus ActionStatus { get; set; }
        public long? Uln { get; set; }
        public required string FamilyName { get; set; }
        public required string GivenNames { get; set; }
        public Guid? CertificateId { get; set; }
        public CertificateType CertificateType { get; set; }
        public string CourseName { get; set; }
        public List<UserMatch> UserMatches { get; set; }
        public List<AdminAction> AdminActions { get; set; }
    }
}

using System;
using System.Collections.Generic;
using SFA.DAS.AdminService.Common.Models;

namespace SFA.DAS.AdminService.Web.ViewModels.Search
{
    public class DigitalAccessReferenceViewModel
    {
        public string ReferenceNumber { get; set; } = string.Empty;
        public UserActionResponse Result { get; set; }
    }

    public class UserActionResponse
    {
        public long Id { get; set; }
        public Guid UserId { get; set; }
        public ActionType ActionType { get; set; }
        public DateTime ActionTime { get; set; }
        public UserActionStatus ActionStatus { get; set; }
        public long? Uln { get; set; }
        public string FamilyName { get; set; }
        public string GivenNames { get; set; }
        public Guid? CertificateId { get; set; }
        public CertificateType? CertificateType { get; set; }
        public string CourseName { get; set; }
        public List<AdminAction> AdminActions { get; set; }
    }

    public class AdminAction
    {
        public string Username { get; set; }
        public DateTime ActionTime { get; set; }
        public AdminActionType Action { get; set; }
    }
}

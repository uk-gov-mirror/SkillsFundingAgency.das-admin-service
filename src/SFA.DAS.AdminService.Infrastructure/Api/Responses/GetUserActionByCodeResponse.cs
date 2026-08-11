using System;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Infrastructure.Api.Responses
{
    public class GetUserActionByCodeResponse
    {
        public long Id { get; set; }
        public Guid UserId { get; set; }
        public string ActionType { get; set; }
        public DateTime ActionTime { get; set; }
        public string ActionStatus { get; set; }
        public long? Uln { get; set; }
        public string FamilyName { get; set; }
        public string GivenNames { get; set; }
        public Guid? CertificateId { get; set; }
        public int? StandardCode { get; set; }
        public string CertificateType { get; set; }
        public string CourseName { get; set; }
        public List<AdminActionResponse> AdminActions { get; set; }
    }
}

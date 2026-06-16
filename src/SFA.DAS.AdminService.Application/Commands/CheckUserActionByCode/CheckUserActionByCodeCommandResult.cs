using System;
using System.Collections.Generic;
using SFA.DAS.AdminService.Infrastructure.Api.Responses;
using SFA.DAS.AdminService.Common.Models;

namespace SFA.DAS.AdminService.Application.Commands.CheckUserActionByCode
{
    public class CheckUserActionByCodeCommandResult
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
        public List<AdminActionResponse> AdminActions { get; set; }

        public static implicit operator CheckUserActionByCodeCommandResult(CheckUserActionByCodeResponse source)
        {
            if (source == null) return null;

            ActionType parsedActionType;
            if (string.IsNullOrEmpty(source.ActionType) || !Enum.TryParse(source.ActionType, true, out parsedActionType))
            {
                throw new InvalidOperationException($"API response missing or invalid ActionType: '{source?.ActionType}'");
            }

            UserActionStatus parsedStatus;
            if (string.IsNullOrEmpty(source.ActionStatus) || !Enum.TryParse(source.ActionStatus, true, out parsedStatus))
            {
                throw new InvalidOperationException($"API response missing or invalid ActionStatus: '{source?.ActionStatus}'");
            }

            CertificateType? parsedCertificateType = null;
            if (!string.IsNullOrEmpty(source.CertificateType) && Enum.TryParse<CertificateType>(source.CertificateType, true, out var ct))
            {
                parsedCertificateType = ct;
            }

            return new CheckUserActionByCodeCommandResult
            {
                Id = source.Id,
                UserId = source.UserId,
                ActionType = parsedActionType,
                ActionTime = source.ActionTime,
                ActionStatus = parsedStatus,
                Uln = source.Uln,
                FamilyName = source.FamilyName,
                GivenNames = source.GivenNames,
                CertificateId = source.CertificateId,
                CertificateType = parsedCertificateType,
                CourseName = source.CourseName,
                AdminActions = source.AdminActions
            };
        }
    }
}

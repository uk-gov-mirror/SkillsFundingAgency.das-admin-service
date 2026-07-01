using System;

namespace SFA.DAS.AdminService.Web.ViewModels.DigitalAccess
{
    public class RestoreAccessViewModel
    {
        public required string ReferenceNumber { get; set; }
        public Guid UserId { get; set; }
        public long UserActionId { get; set; }
    }
}

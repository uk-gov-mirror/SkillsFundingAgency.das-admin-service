using SFA.DAS.AdminService.Common.Models;

namespace SFA.DAS.AdminService.Web.ViewModels.DigitalAccess
{
    public class DigitalAccessReferenceSearchViewModel
    {
        public string ReferenceNumber { get; set; } = string.Empty;
        public ActionType ActionType { get; set; }
    }
}

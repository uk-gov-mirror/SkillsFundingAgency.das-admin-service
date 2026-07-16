using SFA.DAS.AdminService.Common.Models;

namespace SFA.DAS.AdminService.Web.ViewModels.DigitalAccess
{
    public class NonSpecificContactRequestViewModel
    {
        public string ReferenceNumber { get; set; } = string.Empty;
        public string RequestType { get; set; } = "Incorrect details";
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public long? Uln { get; set; }
    }
}

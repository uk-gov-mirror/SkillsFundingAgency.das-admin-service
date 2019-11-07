using System;

namespace SFA.DAS.AdminService.Web.ViewModels.RoatpAssessor
{
    public class RoatpAssessorGatewayTaskListViewModel
    {
        public Guid ApplicationId { get; set; }
        public string Ukprn { get; set; }
        public string Name { get; set; }
        public string ProviderRoute { get; set; }
        public string ApplicationRef { get; set; }
        public DateTime SubmittedAt { get; set; }

        public bool CanDoLegalChecks { get; set; }
        public bool CanDoAddressChecks { get; set; }
    }
}

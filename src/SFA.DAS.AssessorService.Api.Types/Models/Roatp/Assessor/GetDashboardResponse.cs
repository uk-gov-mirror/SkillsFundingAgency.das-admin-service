using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp.Assessor
{
    public class GetDashboardResponse
    {
        public DashboardTab Tab { get; set; }
        public List<Application> NewApplications { get; set; }
        public string SortedBy { get; set; }
        public bool SortedDescending { get; set; }
        public int NewApplicationsCount { get; set; }
        public int InProgressCount { get; set; }
        public List<ProviderType> ProviderTypes { get; set; }
    }
}

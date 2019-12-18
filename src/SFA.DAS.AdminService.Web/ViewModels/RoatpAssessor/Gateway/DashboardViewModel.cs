using SFA.DAS.AssessorService.Api.Types.Models.Roatp;
using SFA.DAS.AssessorService.Api.Types.Models.Roatp.Assessor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AdminService.Web.ViewModels.RoatpAssessor.Gateway
{
    public class DashboardViewModel
    {
        public const string SortDescending = "desc";
        private const string SelectedTabCss = "govuk-tabs__tab--selected";

        public DashboardTab Tab { get; set; }
        public List<DashboardApplication> NewApplications { get; set; }
        public int NewApplicationsCount { get; set; }
        public int InProgressCount { get; set; }
        public string SortedBy { get; set; }
        public bool SortedDescending { get; set; }

        public string NewApplicationsTabCss => Tab == DashboardTab.New
            ? SelectedTabCss : "";

        public string InProgressTabCss => Tab == DashboardTab.InProgress
            ? SelectedTabCss : "";

        public string OutcomesTabCss => Tab == DashboardTab.Outcomes
            ? SelectedTabCss : "";


        public string SearchTabCss => Tab == DashboardTab.Search
            ? SelectedTabCss : "";

        public string GetRouteSort(string sortBy)
        {
            if (sortBy != SortedBy)
                return null;

            return SortedDescending ? null : SortDescending;
        }

        public string GetAriaSort(string sortBy)
        {
            if (sortBy != SortedBy)
                return null;

            return SortedDescending ? "descending" : "ascending";
        }

        public List<ProviderType> ProviderTypes { get; set; }

        public string ProviderRoute(string applicationRouteId)
        {
            var providerRoute = ProviderTypes.FirstOrDefault(p => p.Id.ToString() == applicationRouteId);

            var routeName = providerRoute.Type;

            return routeName.Substring(0, routeName.IndexOf(' '));
        }
    }

    public class DashboardApplication
    {
        public Guid ApplicationId { get; set; }
        public string OrganisationName { get; set; }
        public string UKPRN { get; set; }
        public string ReferenceNumber { get; set; }
        public string ApplicationRouteId { get; set; }
        public DateTime ApplicationSubmittedOn { get; set; }
    }
}

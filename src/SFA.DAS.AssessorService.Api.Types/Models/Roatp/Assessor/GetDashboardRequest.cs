using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp.Assessor
{ 
    public class GetDashboardRequest : IRequest<GetDashboardResponse>
    {
        public DashboardTab Tab { get; }
        public bool SortDescending { get; }
        public string SortBy { get; }

        public GetDashboardRequest(DashboardTab tab, string sortBy, bool sortDescending)
        {
            Tab = tab;
            SortDescending = sortDescending;
            SortBy = sortBy;
        }
    }
}

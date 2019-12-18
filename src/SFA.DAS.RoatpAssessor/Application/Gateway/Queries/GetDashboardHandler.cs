using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Roatp.Assessor;
using RoatpAssessorTypes = SFA.DAS.AssessorService.Api.Types.Models.Roatp.Assessor;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Queries
{
    public class GetDashboardHandler : IRequestHandler<GetDashboardRequest, GetDashboardResponse>
    {
        private readonly IRoatpApplyApiClient _applyApiClient;

        private readonly IRoatpApiClient _roatpApiClient;

        public GetDashboardHandler(IRoatpApplyApiClient applyApiClient, IRoatpApiClient roatpApiClient)
        {
            _applyApiClient = applyApiClient;
            _roatpApiClient = roatpApiClient;
        }

        public async Task<GetDashboardResponse> Handle(GetDashboardRequest request, CancellationToken cancellationToken)
        {
            var countsTask = _applyApiClient.GetGatewayCounts();

            var newApplicationsTask = request.Tab == DashboardTab.New 
                ? _applyApiClient.GetSubmittedApplications() 
                : Task.FromResult(new List<RoatpAssessorTypes.Application>());

            var providerTypesTask = _roatpApiClient.GetProviderTypes();

            await Task.WhenAll(countsTask, newApplicationsTask, providerTypesTask);

            var newApplications = SortApplications(newApplicationsTask.Result, request.SortBy, request.SortDescending);

            var response = new GetDashboardResponse
            {
                Tab = request.Tab,
                NewApplicationsCount = countsTask.Result.NewApplications,
                InProgressCount = countsTask.Result.InProgress,
                NewApplications = newApplications,
                SortedBy = request.SortBy ?? nameof(RoatpAssessorTypes.Application.ApplicationSubmittedOn),
                SortedDescending = request.SortDescending,
                ProviderTypes = providerTypesTask.Result.ToList()
            };
           
            return response;
        }

        private List<RoatpAssessorTypes.Application> SortApplications(List<RoatpAssessorTypes.Application> applications, string sortBy, bool sortDescending)
        {
            if (applications == null)
                return null;

            switch (sortBy)
            {
                case nameof(RoatpAssessorTypes.Application.OrganisationName):
                    return sortDescending
                        ? applications.OrderByDescending(a => a.OrganisationName).ToList()
                        : applications.OrderBy(a => a.OrganisationName).ToList();

                case nameof(RoatpAssessorTypes.Application.UKPRN):
                    return sortDescending
                        ? applications.OrderByDescending(a => a.UKPRN).ToList()
                        : applications.OrderBy(a => a.UKPRN).ToList();

                case nameof(RoatpAssessorTypes.Application.ReferenceNumber):
                    return sortDescending
                        ? applications.OrderByDescending(a => a.ReferenceNumber).ToList()
                        : applications.OrderBy(a => a.ReferenceNumber).ToList();

                case nameof(RoatpAssessorTypes.Application.ApplicationRouteId):
                    return sortDescending
                        ? applications.OrderByDescending(a => a.ApplicationRouteId).ToList()
                        : applications.OrderBy(a => a.ApplicationRouteId).ToList();

                case nameof(RoatpAssessorTypes.Application.ApplicationSubmittedOn):
                default:
                    return sortDescending
                        ? applications.OrderByDescending(a => a.ApplicationSubmittedOn).ToList()
                        : applications.OrderBy(a => a.ApplicationSubmittedOn).ToList();
            }
        }
    }
}

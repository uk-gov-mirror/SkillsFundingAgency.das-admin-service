using System.Collections.Generic;
using System.Threading.Tasks;
using RoatpAssessorTypes = SFA.DAS.AssessorService.Api.Types.Models.Roatp.Assessor;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IRoatpApplyApiClient
    {
        Task<List<RoatpAssessorTypes.Application>> GetSubmittedApplications();
        Task<RoatpAssessorTypes.GatewayCounts> GetGatewayCounts();
    }
}

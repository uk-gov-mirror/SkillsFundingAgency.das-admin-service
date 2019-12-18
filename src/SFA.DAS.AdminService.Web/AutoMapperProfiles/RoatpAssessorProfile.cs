using AutoMapper;
using SFA.DAS.AdminService.Web.ViewModels.RoatpAssessor.Gateway;
using RoatpAssessorTypes = SFA.DAS.AssessorService.Api.Types.Models.Roatp.Assessor;

namespace SFA.DAS.AdminService.Web.AutoMapperProfiles
{
    public class RoatpAssessorProfile : Profile
    {
        public RoatpAssessorProfile()
        {
            CreateMap<RoatpAssessorTypes.Application, DashboardApplication>();
        }
    }
}

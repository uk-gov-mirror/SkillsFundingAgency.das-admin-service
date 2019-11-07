using AutoMapper;
using SFA.DAS.AdminService.Web.ViewModels.RoatpAssessor;
using SFA.DAS.RoatpAssessor.Application.Gateway.Requests;

namespace SFA.DAS.AdminService.Web.AutoMapperProfiles
{
    public class RoatpAssessorProfile : Profile
    {
        public RoatpAssessorProfile()
        {
            CreateMap<GetGatewayDashboardResponse, RoatpAssessorGatewayDashboardViewModel>();
            CreateMap<GetGatewayTaskListResponse, RoatpAssessorGatewayTaskListViewModel>();
        }
    }
}

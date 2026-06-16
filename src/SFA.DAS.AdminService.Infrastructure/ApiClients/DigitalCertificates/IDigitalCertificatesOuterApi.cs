using RestEase;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Infrastructure.Api.Responses;
using SFA.DAS.AdminService.Infrastructure.Api.Requests;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.DigitalCertificates
{
    public interface IDigitalCertificatesOuterApi
    {
        [Post("/users/useractions/{code}/search")]
        Task<CheckUserActionByCodeResponse> CheckUserActionByCode([Path] string code, [Body] CheckUserActionByCodeRequest request);
    }
}

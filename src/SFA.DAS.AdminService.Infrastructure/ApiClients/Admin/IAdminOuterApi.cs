using RestEase;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Infrastructure.Api.Responses;
using SFA.DAS.AdminService.Infrastructure.Api.Requests;
using System;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.Admin
{
    public interface IAdminOuterApi
    {
        [Post("/user-actions/{code}/search")]
        Task<CheckUserActionByCodeResponse> CheckUserActionByCode([Path] string code, [Body] CheckUserActionByCodeRequest request);
        [Get("/user-actions/{code}/all-activity")]
        Task<UserAllActivityResponse> GetUserAllActivity([Path] string code);
        [Post("/users/{userId}/unlock")]
        Task UnlockUser([Path] Guid userId, [Body] UnlockUserRequest request);
        [Get("/user-actions/{code}")]
        Task<GetUserActionByCodeResponse> GetUserActionByCode([Path] string code);
    }
}

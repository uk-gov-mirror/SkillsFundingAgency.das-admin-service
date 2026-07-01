using RestEase;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Infrastructure.Api.Responses;
using SFA.DAS.AdminService.Infrastructure.Api.Requests;
using System;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.Admin
{
    public interface IAdminOuterApi
    {
        [Post("/users/useractions/{code}/search")]
        Task<CheckUserActionByCodeResponse> CheckUserActionByCode([Path] string code, [Body] CheckUserActionByCodeRequest request);
        [Get("/users/useractions/{code}/all-activity")]
        Task<UserAllActivityResponse> GetUserAllActivity([Path] string code);
        [Post("/users/{userId}/unlock")]
        Task UnlockUser([Path] Guid userId, [Body] UnlockUserRequest request);
    }
}

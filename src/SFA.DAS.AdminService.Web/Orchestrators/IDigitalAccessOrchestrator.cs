using System;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.ViewModels.DigitalAccess;

namespace SFA.DAS.AdminService.Web.Orchestrators
{
    public interface IDigitalAccessOrchestrator
    {
        Task<DigitalAccessReferenceSearchViewModel> GetDigitalAccessReferenceViewModel(string reference, string username);
        Task<UserNotFoundViewModel> GetUserNotFoundViewModel(string reference, string username);
        Task<UserNotMatchedViewModel> GetUserNotMatchedViewModel(string reference);
        Task<CertificateChangeRequestViewModel> GetCertificateChangeRequestViewModel(string reference, string username);
        Task<CertificatePrintRequestViewModel> GetCertificatePrintRequestViewModel(string reference, string username);
        Task<RestoreAccessViewModel> GetRestoreAccessViewModel(string reference);
        Task UnlockUser(Guid userId, string username, long userActionId);
    }
}

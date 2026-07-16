using System;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.ViewModels.DigitalAccess;

namespace SFA.DAS.AdminService.Web.Orchestrators
{
    public interface IDigitalAccessOrchestrator
    {
        Task<DigitalAccessReferenceSearchViewModel> GetDigitalAccessReferenceViewModel(string reference, string username);
        Task<UserNotFoundViewModel> GetUserNotFoundViewModel(string reference);
        Task<UserNotMatchedViewModel> GetUserNotMatchedViewModel(string reference);
        Task<CertificateChangeRequestViewModel> GetCertificateChangeRequestViewModel(string reference);
        Task<NonSpecificContactRequestViewModel> GetNonSpecificContactRequestViewModel(string reference);
        Task<CertificatePrintRequestViewModel> GetCertificatePrintRequestViewModel(string reference);
        Task<RestoreAccessViewModel> GetRestoreAccessViewModel(string reference);
        Task UnlockUser(Guid userId, string username, long userActionId);
    }
}

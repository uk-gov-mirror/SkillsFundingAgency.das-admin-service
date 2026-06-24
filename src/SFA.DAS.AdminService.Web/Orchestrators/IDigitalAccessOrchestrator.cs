using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.ViewModels.DigitalAccess;

namespace SFA.DAS.AdminService.Web.Orchestrators
{
    public interface IDigitalAccessOrchestrator
    {
        Task<DigitalAccessReferenceSearchViewModel> GetDigitalAccessReferenceViewModel(string reference, string username);
        Task<UserNotFoundViewModel> GetUserNotFoundViewModel(string reference, string username);
        Task<UserNotMatchedViewModel> GetUserNotMatchedViewModel(string reference);
    }
}

using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.ViewModels.Search;

namespace SFA.DAS.AdminService.Web.Orchestrators
{
    public interface IDigitalAccessOrchestrator
    {
        Task<DigitalAccessReferenceSearchViewModel> GetDigitalAccessReferenceViewModel(string reference, string username);
        Task<UserNotFoundViewModel> GetUserNotFoundViewModel(string reference, string username);
    }
}

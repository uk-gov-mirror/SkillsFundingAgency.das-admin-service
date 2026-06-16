using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.ViewModels.Search;

namespace SFA.DAS.AdminService.Web.Orchestrators
{
    public interface ISearchOrchestrator
    {
        Task<DigitalAccessReferenceViewModel> FindUserActionByReference(string reference, string username);
    }
}

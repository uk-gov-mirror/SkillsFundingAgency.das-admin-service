using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SFA.DAS.AdminService.Web.Orchestrators;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Web.ViewModels.DigitalAccess;
using SFA.DAS.AdminService.Common.Models;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize(Roles = Domain.Roles.OperationsTeam + "," + Domain.Roles.CertificationTeam)]
    public class DigitalAccessController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IDigitalAccessOrchestrator _orchestrator;

        public const string UserNotFoundRouteGet = nameof(UserNotFoundRouteGet);
        public const string UserNotMatchedRouteGet = nameof(UserNotMatchedRouteGet);
        public const string DigitalAccessReferenceSearchRouteGet = nameof(DigitalAccessReferenceSearchRouteGet);
        public const string DigitalAccessReferenceSearchRoutePost = nameof(DigitalAccessReferenceSearchRoutePost);

        public DigitalAccessController(IHttpContextAccessor contextAccessor, IDigitalAccessOrchestrator orchestrator)
        {
            _contextAccessor = contextAccessor;
            _orchestrator = orchestrator;
        }

        [HttpGet("digital-access/reference", Name = DigitalAccessReferenceSearchRouteGet)]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        public IActionResult DigitalAccessReferenceSearch()
        {
            return View(new DigitalAccessReferenceSearchViewModel());
        }

        [HttpPost("digital-access/reference", Name = DigitalAccessReferenceSearchRoutePost)]
        [ModelStatePersist(ModelStatePersist.Store)]
        public async Task<IActionResult> DigitalAccessReferenceSearch(DigitalAccessReferenceSearchViewModel vm, int page = 1)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToRoute(DigitalAccessReferenceSearchRouteGet);
            }

            var username = _contextAccessor.HttpContext.User.UserDisplayName();

            var resultVm = await _orchestrator.GetDigitalAccessReferenceViewModel(vm.ReferenceNumber, username);

            if (resultVm == null)
            {
                ModelState.AddModelError("ReferenceNumber", "No records found with this reference number");
                return RedirectToRoute(DigitalAccessReferenceSearchRouteGet);
            }

            switch (resultVm.ActionType)
            {
                case ActionType.Reprint:
                case ActionType.Help:
                case ActionType.Contact:
                    return View(resultVm);
                case ActionType.NotFound:
                    return RedirectToRoute(UserNotFoundRouteGet, new { referenceNumber = resultVm.ReferenceNumber });
                case ActionType.NotMatched:
                    return RedirectToRoute(UserNotMatchedRouteGet, new { referenceNumber = resultVm.ReferenceNumber });
            }

            return View(resultVm);
        }

        [HttpGet("digital-access/reference/{referenceNumber}/not-found", Name = UserNotFoundRouteGet)]
        public async Task<IActionResult> UserNotFound(string referenceNumber)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var vm = await _orchestrator.GetUserNotFoundViewModel(referenceNumber, username);
            if (vm == null)
            {
                return RedirectToRoute(DigitalAccessReferenceSearchRouteGet);
            }

            return View(vm);
        }

        [HttpGet("digital-access/reference/{referenceNumber}/not-matched", Name = UserNotMatchedRouteGet)]
        public async Task<IActionResult> UserNotMatched(string referenceNumber)
        {
            var vm = await _orchestrator.GetUserNotMatchedViewModel(referenceNumber);
            if (vm == null)
            {
                return RedirectToRoute(DigitalAccessReferenceSearchRouteGet);
            }

            return View(vm);
        }

        
    }
}

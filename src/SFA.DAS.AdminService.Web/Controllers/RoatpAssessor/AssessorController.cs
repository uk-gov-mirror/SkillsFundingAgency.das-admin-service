using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Web.ViewModels.RoatpAssessor.Gateway;
using System;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Roatp.Assessor;

namespace SFA.DAS.AdminService.Web.Controllers.RoatpAssessor
{
    [Authorize(Roles = Domain.Roles.RoatpAssessorGateway + "," + Domain.Roles.RoatpAssessorTeam)]
    [Route("roatp-assessor/assessor")]
    public class AssessorController : Controller
    {
        private readonly IMediator _mediator;

        public AssessorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("dashboard", Name = RouteNames.RoatpAssessor_Assessor_Dashboard_Get)]
        public async Task<IActionResult> Dashboard([FromQuery]string tab, [FromQuery] string sortBy, [FromQuery] string sort)
        {
            if (!Enum.TryParse(tab, out DashboardTab selectedTab))
                selectedTab = DashboardTab.New;

            var sortDescending = sort == DashboardViewModel.SortDescending;
            var request = new GetDashboardRequest(selectedTab, sortBy, sortDescending);
            var response = await _mediator.Send(request);

            var vm = Mapper.Map<DashboardViewModel>(response);

            return View(vm);
        }

    }
}
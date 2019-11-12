using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Applications;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AdminService.Web.Helpers;
using SFA.DAS.AdminService.Web.Extensions.TagHelpers;
using SFA.DAS.AdminService.Web.Extensions;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using System;
using SFA.DAS.AdminService.Web.Domain.Apply;
using System.Linq;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.AdminService.Web.Controllers.Apply
{
    [Authorize(Roles = Roles.AssessmentDeliveryTeam + "," + Roles.CertificationTeam)]
    [CheckSession(nameof(StandardApplicationController), nameof(ResetSession), nameof(IControllerSession.StandardApplication_SessionValid))]
    public class StandardApplicationController : Controller
    {
        private readonly IControllerSession _controllerSession;
        private readonly IApiClient _apiClient;
        private readonly IApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly ILogger<StandardApplicationController> _logger;

        private const int DefaultPageIndex = 1;
        private const int DefaultApplicationsPerPage = 10;
        private const int DefaultPageSetSize = 6;

        public StandardApplicationController(IControllerSession controllerSession, IApiClient apiClient, IApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient, ILogger<StandardApplicationController> logger)
        {
            _apiClient = apiClient;
            _controllerSession = controllerSession;
            _qnaApiClient = qnaApiClient;
            _applyApiClient = applyApiClient;
            _logger = logger;
        }

        [HttpGet]
        [CheckSession(nameof(IControllerSession.StandardApplication_SessionValid), CheckSession.Ignore)]
        public IActionResult ResetSession()
        {
            SetDefaultSession();
            return RedirectToAction(nameof(StandardApplications));
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(StandardApplications));
        }

        [HttpGet("/Applications/{applicationId}/Standard")]
        public async Task<IActionResult> ActiveSequence(Guid applicationId)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var organisation = await _apiClient.GetOrganisation(application.OrganisationId);

            var activeApplySequence = application.ApplyData.Sequences.Where(seq => seq.IsActive && !seq.NotRequired).OrderBy(seq => seq.SequenceNo).FirstOrDefault();

            var sequence = await _qnaApiClient.GetSequence(application.ApplicationId, activeApplySequence.SequenceId);
            var sections = await _qnaApiClient.GetSections(application.ApplicationId, sequence.Id);

            var sequenceVm = new SequenceViewModel(application, ApplyConst.STANDARD_APPLICATION_TYPE, organisation, sequence, sections,
                activeApplySequence.Sections, GetPagingState(application.ReviewStatus)?.PageIndex);

            return View(nameof(Sequence), sequenceVm);
        }

        [HttpGet("/Applications/{applicationId}/Standard/Sequence/{sequenceNo}")]
        public async Task<IActionResult> Sequence(Guid applicationId, int sequenceNo)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            var organisation = await _apiClient.GetOrganisation(application.OrganisationId);

            var applySequence = application.ApplyData.Sequences.Single(x => x.SequenceNo == sequenceNo);

            var sequence = await _qnaApiClient.GetSequence(application.ApplicationId, applySequence.SequenceId);
            var sections = await _qnaApiClient.GetSections(application.ApplicationId, sequence.Id);

            var sequenceVm = new SequenceViewModel(application, ApplyConst.STANDARD_APPLICATION_TYPE, organisation, sequence, sections,
                applySequence.Sections, GetPagingState(application.ReviewStatus)?.PageIndex);

            if (application.ApplicationStatus == ApplicationStatus.Submitted || application.ApplicationStatus == ApplicationStatus.Resubmitted)
            {
                return View(nameof(Sequence), sequenceVm);
            }
            else
            {
                return View($"{nameof(Sequence)}_ReadOnly", sequenceVm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> StandardApplications(int pageIndex = 1)
        {
            // reset only the page indexes; retain the sort column, direction and applications per page settings
            _controllerSession.StandardApplication_NewApplications.PageIndex = 1;
            _controllerSession.StandardApplication_InProgressApplictions.PageIndex = 1;
            _controllerSession.StandardApplication_FeedbackApplications.PageIndex = 1;
            _controllerSession.StandardApplication_ApprovedApplications.PageIndex = 1;

            var vm = await MapViewModelFromSession();
            return View(nameof(StandardApplications), vm);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePageNewApplications(int pageIndex = DefaultPageIndex)
        {
            return await ChangePageApplications(ApplicationReviewStatus.New, pageIndex);
        }

        [HttpGet]
        [CheckSession(nameof(IControllerSession.StandardApplication_SessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangePageNewApplicationsPartial(int pageIndex = DefaultPageIndex)
        {
            return await ChangePageApplicationsPartial(ApplicationReviewStatus.New, pageIndex);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeApplicationsPerPageNewApplications(int itemsPerPage = DefaultApplicationsPerPage)
        {
            return await ChangeApplicationsPerPageApplications(ApplicationReviewStatus.New, itemsPerPage);
        }

        [HttpGet]
        [CheckSession(nameof(IControllerSession.StandardApplication_SessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangeApplicationsPerPageNewApplicationsPartial(int itemsPerPage = DefaultApplicationsPerPage)
        {
            return await ChangeApplicationsPerPageApplicationsPartial(ApplicationReviewStatus.New, itemsPerPage);
        }

        [HttpGet]
        public async Task<IActionResult> SortNewApplications(string sortColumn, string sortDirection)
        {
            return await SortApplications(ApplicationReviewStatus.New, sortColumn, sortDirection);
        }

        [HttpGet]
        [CheckSession(nameof(IControllerSession.StandardApplication_SessionValid), CheckSession.Error)]
        public async Task<IActionResult> SortNewApplicationsPartial(string sortColumn, string sortDirection)
        {
            return await SortApplicationsPartial(ApplicationReviewStatus.New, sortColumn, sortDirection);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePageInProgressApplications(int pageIndex = DefaultPageIndex)
        {
            return await ChangePageApplications(ApplicationReviewStatus.InProgress, pageIndex);
        }

        [HttpGet]
        [CheckSession(nameof(IControllerSession.StandardApplication_SessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangePageInProgressApplicationsPartial(int pageIndex = DefaultPageIndex)
        {
            return await ChangePageApplicationsPartial(ApplicationReviewStatus.InProgress, pageIndex);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeApplicationsPerPageInProgressApplications(int itemsPerPage = DefaultApplicationsPerPage)
        {
            return await ChangeApplicationsPerPageApplications(ApplicationReviewStatus.InProgress, itemsPerPage);
        }

        [HttpGet]
        [CheckSession(nameof(IControllerSession.StandardApplication_SessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangeApplicationsPerPageInProgressApplicationsPartial(int itemsPerPage = DefaultApplicationsPerPage)
        {
            return await ChangeApplicationsPerPageApplicationsPartial(ApplicationReviewStatus.InProgress, itemsPerPage);
        }

        [HttpGet]
        public async Task<IActionResult> SortInProgressApplications(string sortColumn, string sortDirection)
        {
            return await SortApplications(ApplicationReviewStatus.InProgress, sortColumn, sortDirection);
        }

        [HttpGet]
        [CheckSession(nameof(IControllerSession.StandardApplication_SessionValid), CheckSession.Error)]
        public async Task<IActionResult> SortInProgressApplicationsPartial(string sortColumn, string sortDirection)
        {
            return await SortApplicationsPartial(ApplicationReviewStatus.InProgress, sortColumn, sortDirection);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePageFeedbackApplications(int pageIndex = DefaultPageIndex)
        {
            return await ChangePageApplications(ApplicationReviewStatus.HasFeedback, pageIndex);
        }

        [HttpGet]
        [CheckSession(nameof(IControllerSession.StandardApplication_SessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangePageFeedbackApplicationsPartial(int pageIndex = DefaultPageIndex)
        {
            return await ChangePageApplicationsPartial(ApplicationReviewStatus.HasFeedback, pageIndex);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeApplicationsPerPageFeedbackApplications(int itemsPerPage = DefaultApplicationsPerPage)
        {
            return await ChangeApplicationsPerPageApplications(ApplicationReviewStatus.HasFeedback, itemsPerPage);
        }

        [HttpGet]
        [CheckSession(nameof(IControllerSession.StandardApplication_SessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangeApplicationsPerPageFeedbackApplicationsPartial(int itemsPerPage = DefaultApplicationsPerPage)
        {
            return await ChangeApplicationsPerPageApplicationsPartial(ApplicationReviewStatus.HasFeedback, itemsPerPage);
        }

        [HttpGet]
        public async Task<IActionResult> SortFeedbackApplications(string sortColumn, string sortDirection)
        {
            return await SortApplications(ApplicationReviewStatus.HasFeedback, sortColumn, sortDirection);
        }

        [HttpGet]
        [CheckSession(nameof(IControllerSession.StandardApplication_SessionValid), CheckSession.Error)]
        public async Task<IActionResult> SortFeedbackApplicationsPartial(string sortColumn, string sortDirection)
        {
            return await SortApplicationsPartial(ApplicationReviewStatus.HasFeedback, sortColumn, sortDirection);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePageApprovedApplications(int pageIndex = DefaultPageIndex)
        {
            return await ChangePageApplications(ApplicationReviewStatus.Approved, pageIndex);
        }

        [HttpGet]
        [CheckSession(nameof(IControllerSession.StandardApplication_SessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangePageApprovedApplicationsPartial(int pageIndex = DefaultPageIndex)
        {
            return await ChangePageApplicationsPartial(ApplicationReviewStatus.Approved, pageIndex);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeApplicationsPerPageApprovedApplications(int itemsPerPage = DefaultApplicationsPerPage)
        {
            return await ChangeApplicationsPerPageApplications(ApplicationReviewStatus.Approved, itemsPerPage);
        }

        [HttpGet]
        [CheckSession(nameof(IControllerSession.StandardApplication_SessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangeApplicationsPerPageApprovedApplicationsPartial(int itemsPerPage = DefaultApplicationsPerPage)
        {
            return await ChangeApplicationsPerPageApplicationsPartial(ApplicationReviewStatus.Approved, itemsPerPage);
        }

        [HttpGet]
        public async Task<IActionResult> SortApprovedApplications(string sortColumn, string sortDirection)
        {
            return await SortApplications(ApplicationReviewStatus.Approved, sortColumn, sortDirection);
        }

        [HttpGet]
        [CheckSession(nameof(IControllerSession.StandardApplication_SessionValid), CheckSession.Error)]
        public async Task<IActionResult> SortApprovedApplicationsPartial(string sortColumn, string sortDirection)
        {
            return await SortApplicationsPartial(ApplicationReviewStatus.Approved, sortColumn, sortDirection);
        }

        private IPagingState GetPagingState(string reviewStatus)
        {
            switch (reviewStatus)
            {
                case ApplicationReviewStatus.New:
                    return _controllerSession.StandardApplication_NewApplications;
                case ApplicationReviewStatus.InProgress:
                    return _controllerSession.StandardApplication_InProgressApplictions;
                case ApplicationReviewStatus.HasFeedback:
                    return _controllerSession.StandardApplication_FeedbackApplications;
                case ApplicationReviewStatus.Approved:
                    return _controllerSession.StandardApplication_ApprovedApplications;
            }

            return null;
        }

        private async Task<ApplicationsDashboardViewModel> MapViewModelFromSession()
        {
            var viewModel = new ApplicationsDashboardViewModel(nameof(StandardApplicationController).RemoveController());

            viewModel.NewApplications = await AddApplicationsViewModelValues(viewModel.NewApplications, ApplicationReviewStatus.New, _controllerSession.StandardApplication_NewApplications);
            viewModel.InProgressApplications = await AddApplicationsViewModelValues(viewModel.InProgressApplications, ApplicationReviewStatus.InProgress, _controllerSession.StandardApplication_InProgressApplictions);
            viewModel.FeedbackApplications = await AddApplicationsViewModelValues(viewModel.FeedbackApplications, ApplicationReviewStatus.HasFeedback, _controllerSession.StandardApplication_FeedbackApplications);
            viewModel.ApprovedApplications = await AddApplicationsViewModelValues(viewModel.ApprovedApplications, ApplicationReviewStatus.Approved, _controllerSession.StandardApplication_ApprovedApplications);

            return viewModel;
        }

        private async Task<IActionResult> ChangePageApplications(string reviewStatus, int pageIndex)
        {
            var pagingState = GetPagingState(reviewStatus);
            pagingState.PageIndex = pageIndex;

            var vm = await MapViewModelFromSession();
            return View(nameof(StandardApplications), vm);
        }

        private async Task<IActionResult> ChangePageApplicationsPartial(string reviewStatus, int pageIndex)
        {
            var pagingState = this.GetPagingState(reviewStatus);
            pagingState.PageIndex = pageIndex;

            var viewModel = await AddApplicationsViewModelValues(new ApplicationsViewModel(), reviewStatus, pagingState);
            return PartialView("_StandardApplicationsPartial", viewModel);
        }

        private async Task<IActionResult> ChangeApplicationsPerPageApplications(string reviewStatus, int itemsPerPage)
        {
            var pagingState = this.GetPagingState(reviewStatus);
            pagingState.ItemsPerPage = itemsPerPage;

            return await ChangePageApplications(reviewStatus, 1);
        }

        private async Task<IActionResult> ChangeApplicationsPerPageApplicationsPartial(string reviewStatus, int itemsPerPage)
        {
            var pagingState = GetPagingState(reviewStatus);
            pagingState.ItemsPerPage = itemsPerPage;

            return await ChangePageApplicationsPartial(reviewStatus, 1);
        }

        private async Task<IActionResult> SortApplications(string reviewStatus, string sortColumn, string sortDirection)
        {
            UpdateSortDirection(sortColumn, sortDirection, GetPagingState(reviewStatus));
            return await ChangePageApplications(reviewStatus, 1);
        }

        private async Task<IActionResult> SortApplicationsPartial(string reviewStatus, string sortColumn, string sortDirection)
        {
            UpdateSortDirection(sortColumn, sortDirection, GetPagingState(reviewStatus));
            return await ChangePageApplicationsPartial(reviewStatus, 1);
        }

        private void UpdateSortDirection(string sortColumn, string sortDirection, IPagingState pagingState)
        {
            if (pagingState.SortColumn == sortColumn)
            {
                pagingState.SortDirection = sortDirection;
            }
            else
            {
                pagingState.SortColumn = sortColumn;
                pagingState.SortDirection = SortOrder.Asc;
            }
        }

        private async Task<ApplicationsViewModel> AddApplicationsViewModelValues(ApplicationsViewModel viewModel, string reviewStatus, IPagingState pagingState)
        {
            viewModel.ReviewStatus = reviewStatus;
            viewModel.PageIndex = pagingState.PageIndex;
            viewModel.ItemsPerPage = pagingState.ItemsPerPage;
            viewModel.SortColumn = pagingState.SortColumn;
            viewModel.SortDirection = pagingState.SortDirection;
            viewModel.PaginatedList = await GetPageApplications(viewModel.ReviewStatus, pagingState);

            switch (reviewStatus)
            {
                case ApplicationReviewStatus.New:
                    viewModel.ChangePageAction = nameof(ChangePageNewApplications);
                    viewModel.ChangeItemsPerPageAction = nameof(ChangeApplicationsPerPageNewApplications);
                    viewModel.SortColumnAction = nameof(SortNewApplications);
                    viewModel.Title = "New";
                    break;
                case ApplicationReviewStatus.InProgress:
                    viewModel.ChangePageAction = nameof(ChangePageInProgressApplications);
                    viewModel.ChangeItemsPerPageAction = nameof(ChangeApplicationsPerPageInProgressApplications);
                    viewModel.SortColumnAction = nameof(SortInProgressApplications);
                    viewModel.Title = "In progress";
                    break;
                case ApplicationReviewStatus.HasFeedback:
                    viewModel.ChangePageAction = nameof(ChangePageFeedbackApplications);
                    viewModel.ChangeItemsPerPageAction = nameof(ChangeApplicationsPerPageFeedbackApplications);
                    viewModel.SortColumnAction = nameof(SortFeedbackApplications);
                    viewModel.Title = "Feedback";
                    break;
                case ApplicationReviewStatus.Approved:
                    viewModel.ChangePageAction = nameof(ChangePageApprovedApplications);
                    viewModel.ChangeItemsPerPageAction = nameof(ChangeApplicationsPerPageApprovedApplications);
                    viewModel.SortColumnAction = nameof(SortApprovedApplications);
                    viewModel.Title = "Approved";
                    break;
            }

            viewModel.Fragment = ApplicationReviewHelpers.ApplicationFragment(reviewStatus);

            return viewModel;
        }

        private async Task<PaginatedList<ApplicationSummaryItem>> GetPageApplications(string reviewStatus, IPagingState pagingState)
        {
            var standardApplicationsRequest = new StandardApplicationsRequest
            (
                reviewStatus,
                pagingState.SortColumn.ToString(),
                pagingState.SortDirection == SortOrder.Asc ? 1 : 0,
                pagingState.ItemsPerPage,
                pagingState.PageIndex,
                DefaultPageSetSize
            );

            var response = await _applyApiClient.GetStandardApplications(standardApplicationsRequest);
            return response;
        }

        private void SetDefaultSession()
        {
            _controllerSession.StandardApplication_SessionValid = true;

            _controllerSession.StandardApplication_NewApplications.PageIndex = DefaultPageIndex;
            _controllerSession.StandardApplication_NewApplications.ItemsPerPage = DefaultApplicationsPerPage;
            _controllerSession.StandardApplication_NewApplications.SortColumn = StandardApplicationsSortColumn.SubmittedDate;
            _controllerSession.StandardApplication_NewApplications.SortDirection = SortOrder.Desc;

            _controllerSession.StandardApplication_InProgressApplictions.PageIndex = DefaultPageIndex;
            _controllerSession.StandardApplication_InProgressApplictions.ItemsPerPage = DefaultApplicationsPerPage;
            _controllerSession.StandardApplication_InProgressApplictions.SortColumn = StandardApplicationsSortColumn.SubmittedDate;
            _controllerSession.StandardApplication_InProgressApplictions.SortDirection = SortOrder.Desc;

            _controllerSession.StandardApplication_FeedbackApplications.PageIndex = DefaultPageIndex;
            _controllerSession.StandardApplication_FeedbackApplications.ItemsPerPage = DefaultApplicationsPerPage;
            _controllerSession.StandardApplication_FeedbackApplications.SortColumn = StandardApplicationsSortColumn.FeedbackAddedDate;
            _controllerSession.StandardApplication_FeedbackApplications.SortDirection = SortOrder.Desc;

            _controllerSession.StandardApplication_ApprovedApplications.PageIndex = DefaultPageIndex;
            _controllerSession.StandardApplication_ApprovedApplications.ItemsPerPage = DefaultApplicationsPerPage;
            _controllerSession.StandardApplication_ApprovedApplications.SortColumn = StandardApplicationsSortColumn.ClosedDate;
            _controllerSession.StandardApplication_ApprovedApplications.SortDirection = SortOrder.Desc;
        }
    }
}
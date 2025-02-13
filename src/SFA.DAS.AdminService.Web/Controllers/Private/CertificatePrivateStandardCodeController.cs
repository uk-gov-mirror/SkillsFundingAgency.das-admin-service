﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.ExternalApis.Services;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Private;

namespace SFA.DAS.AdminService.Web.Controllers.Private
{
    [Authorize]
    [Route("certificate/privatestandardcodes")]
    public class CertificatePrivateStandardCodeController : CertificateBaseController
    {
        private readonly IOrganisationsApiClient _organisationsApiClient;
        private readonly IStandardServiceClient _standardServiceClient;
        private readonly CacheService _cacheHelper;

        public CertificatePrivateStandardCodeController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            CacheService cacheHelper,
            ApiClient apiClient,
            IStandardServiceClient standardServiceClient, IOrganisationsApiClient organisationsApiClient)
            : base(logger, contextAccessor, apiClient)
        {
            _cacheHelper = cacheHelper;
            _standardServiceClient = standardServiceClient;
            _organisationsApiClient = organisationsApiClient;
        }

        [HttpGet]
        public async Task<IActionResult> StandardCode(Guid certificateId, bool fromApproval)
        {
            var filteredStandardCodes = await GetFilteredStatusCodes(certificateId);
            var standards = (await GetAllStandards()).ToList();

            var results = GetSelectListItems(standards, filteredStandardCodes);

            var viewModel = await LoadViewModel<CertificateStandardCodeListViewModel>(certificateId, "~/Views/CertificateAmend/StandardCode.cshtml");
            if (viewModel is ViewResult viewResult &&
                viewResult.Model is CertificateStandardCodeListViewModel certificateStandardCodeListViewModel)
            {
                certificateStandardCodeListViewModel.FromApproval = fromApproval;
                certificateStandardCodeListViewModel.StandardCodes = results;
            }
            return viewModel;
        }

        [HttpPost(Name = "StandardCode")]
        public async Task<IActionResult> StandardCode(CertificateStandardCodeListViewModel vm)
        {
            var username = ContextAccessor.HttpContext.User
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            var filteredStandardCodes = await GetFilteredStatusCodes(vm.Id);
            var standards = (await GetAllStandards()).ToList();

            vm.StandardCodes = GetSelectListItems(standards, filteredStandardCodes);
            if (!string.IsNullOrEmpty(vm.SelectedStandardCode))
            {
                var selectedStandard = standards.First(q => q.StandardId.ToString() == vm.SelectedStandardCode);
                vm.Standard = selectedStandard.Title;
                vm.Level = selectedStandard.StandardData.Level.GetValueOrDefault();
            }

            var actionResult = await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/StandardCode.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id, fromapproval = vm.FromApproval }), action: CertificateActions.StandardCode);

            return actionResult;
        }

        private IEnumerable<SelectListItem> GetSelectListItems(List<StandardCollation> standards,
            List<string> filteredStandardCodes)
        {
            return standards
                .Where(a => filteredStandardCodes.Contains(a.StandardId.ToString()))
                .Select(q => new SelectListItem { Value = q.StandardId.ToString(), Text = q.Title.ToString() + " (" + q.StandardId + ')' })
                .ToList()
                .OrderBy(q => q.Text);
        }

        private async Task<List<string>> GetFilteredStatusCodes(Guid certificateId)
        {
           var certificate = await ApiClient.GetCertificate(certificateId);
            var organisation = await ApiClient.GetOrganisation(certificate.OrganisationId);         

            var filteredStandardCodes =
                (await _organisationsApiClient
                    .GetOrganisationStandardsByOrganisation(organisation.EndPointAssessorOrganisationId))
                .Select(q => q.StandardCode.ToString()).ToList();
            return filteredStandardCodes;
        }

        private async Task<IEnumerable<StandardCollation>> GetAllStandards()
        {
            var results = await _cacheHelper.RetrieveFromCache<IEnumerable<StandardCollation>>("Standards");
            if (results == null)
            {
                var standards = await _standardServiceClient.GetAllStandards();
                await _cacheHelper.SaveToCache("Standards", standards, 1);

                results = standards;
            }

            return results;
        }
    }
}
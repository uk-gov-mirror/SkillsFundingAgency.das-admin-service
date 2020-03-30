﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Services.Gateway
{
    public class GatewayExperienceAndAccreditationOrchestrator : IGatewayExperienceAndAccreditationOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IRoatpExperienceAndAccreditationApiClient _experienceAndAccreditationApiClient;
        private readonly ILogger<GatewayExperienceAndAccreditationOrchestrator> _logger;

        public GatewayExperienceAndAccreditationOrchestrator(IRoatpApplicationApiClient applyApiClient, IRoatpExperienceAndAccreditationApiClient experienceAndAccreditationApiClient, ILogger<GatewayExperienceAndAccreditationOrchestrator> logger)
        {
            _applyApiClient = applyApiClient;
            _experienceAndAccreditationApiClient = experienceAndAccreditationApiClient;
            _logger = logger;
        }

        public async Task<SubcontractorDeclarationViewModel> GetSubcontractorDeclarationViewModel(GetSubcontractorDeclarationRequest request)
        {
            _logger.LogInformation($"Retrieving subcontractor declaration details for application {request.ApplicationId}");

            var model = new SubcontractorDeclarationViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewayPageIds.SubcontractorDeclaration, request.UserName);

            var subcontractorDeclaration = await _experienceAndAccreditationApiClient.GetSubcontractorDeclaration(request.ApplicationId);

            model.HasDeliveredTrainingAsSubcontractor = subcontractorDeclaration.HasDeliveredTrainingAsSubcontractor;
            model.ContractFileName = subcontractorDeclaration.ContractFileName;

            return model;
        }

        public async Task<FileStreamResult> GetSubcontractorDeclarationContractFile(GetSubcontractorDeclarationContractFileRequest request)
        {
            return await _experienceAndAccreditationApiClient.GetSubcontractorDeclarationContractFile(request.ApplicationId);
        }

        public async Task<OfficeForStudentsViewModel> GetOfficeForStudentsViewModel(GetOfficeForStudentsRequest request)
        {
            _logger.LogInformation($"Retrieving office for students details for application {request.ApplicationId}");

            var model = new OfficeForStudentsViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewayPageIds.OfficeForStudents, request.UserName);

            model.IsOrganisationFundedByOfficeForStudents = await _experienceAndAccreditationApiClient.GetOfficeForStudents(request.ApplicationId) == "Yes";

            return model;
        }
    }
}

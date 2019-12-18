using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AdminService.Web.ViewModels.Roatp;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.AdminService.Web.Validators.Roatp
{
    public class UpdateOrganisationUkprnValidator : IUpdateOrganisationUkprnValidator
    {
        private IRoatpApiClient _apiClient;
  
        public UpdateOrganisationUkprnValidator(IRoatpApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public List<ValidationErrorDetail> IsDuplicateUkprn(UpdateOrganisationUkprnViewModel viewModel)
        {
            var errorMessages = new List<ValidationErrorDetail>();

            long ukprnValue = 0;
            var isParsed = long.TryParse(viewModel.Ukprn, out ukprnValue);

            var duplicateCheckResponse = _apiClient.DuplicateUKPRNCheck(viewModel.OrganisationId, ukprnValue).Result;

            if (duplicateCheckResponse == null || !duplicateCheckResponse.DuplicateFound) return errorMessages;

            var duplicateErrorMessage =
                $"This is an existing UKPRN for '{duplicateCheckResponse.DuplicateOrganisationName}'";
            errorMessages.Add(new ValidationErrorDetail("UKPRN", duplicateErrorMessage));

            return errorMessages;
        }
    }
}

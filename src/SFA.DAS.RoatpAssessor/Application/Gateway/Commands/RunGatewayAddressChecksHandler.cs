using MediatR;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.RoatpAssessor.Application.Extensions;
using SFA.DAS.RoatpAssessor.Application.Mappers;
using SFA.DAS.RoatpAssessor.Application.Services;
using SFA.DAS.RoatpAssessor.Domain.Entities;
using SFA.DAS.RoatpAssessor.Services.ApplyApi;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Commands
{
    public class RunGatewayAddressChecksHandler : IRequestHandler<RunGatewayAddressChecksCommand>
    {
        private readonly IApplyApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IAddressCheckMapper _addressCheckMapper;
        private readonly ITimeProvider _timeProvider;

        public RunGatewayAddressChecksHandler(
            IApplyApiClient applyApiClient, 
            IQnaApiClient qnaApiClient, 
            IAddressCheckMapper addressCheckMapper,
            ITimeProvider timeProvider)
        {
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
            _addressCheckMapper = addressCheckMapper;
            _timeProvider = timeProvider;
        }

        public async Task<Unit> Handle(RunGatewayAddressChecksCommand request, CancellationToken cancellationToken)
        {
            var review = await _applyApiClient.GetApplicationReviewAsync(request.ApplicationId);

            if (!review.GatewayReviewIsInProgress)
            {
                throw new Exception($"Review '{request.ApplicationId}' is not in correct state to run Gateway address checks");
            }

            var applicationData = new RoatpAssessorApplicationData(await _qnaApiClient.GetApplicationData(review.ApplicationId));

            var ukrlpCheckTask = DoUkrlpCheckAsync(applicationData.UKPRN);
            var companyHouseCheckTask = TryCompaniesHouseCheckAsync(applicationData.UKRLP_Verification_CompanyNumber);
            var charityCommisionTask = TryCharityCommissionCheckAsync(applicationData.UKRLP_Verification_CharityRegNumber);

            await Task.WhenAll(ukrlpCheckTask, companyHouseCheckTask, charityCommisionTask);

            var addressChecks = new AddressChecks
            {
                CheckedAt = _timeProvider.UtcNow,
                Ukrlp = ukrlpCheckTask.Result,
                CompaniesHouse = companyHouseCheckTask.Result,
                CharityCommission = charityCommisionTask.Result
            };

            await _applyApiClient.UpdateApplicationReviewAddressChecks(review.ApplicationId, addressChecks);

            return Unit.Value;
        }

        private async Task<AddressCheck> DoUkrlpCheckAsync(string ukprn)
        {
            var response = await _applyApiClient.UkrlpLookup(ukprn);

            if (!response.Success || !response.Results.Any())
                return null;

            var providerDetails = response.Results.FirstOrDefault();

            var check = _addressCheckMapper.MapUkrlp(providerDetails);

            return check;
        }

        private async Task<AddressCheck> TryCompaniesHouseCheckAsync(string companiesHouseNumber)
        {
            if (string.IsNullOrEmpty(companiesHouseNumber))
                return null;

            var company = await _applyApiClient.GetCompanyDetails(companiesHouseNumber);

            if (company == null)
                return null;

            var check = _addressCheckMapper.MapCompaniesHouse(company);

            return check;
        }

        private async Task<AddressCheck> TryCharityCommissionCheckAsync(string charityRegNumber)
        {
            if (!int.TryParse(charityRegNumber, out var charityNo))
                return null;

            var charity = await _applyApiClient.GetCharityDetails(charityNo);

            if (charity == null)
                return null;

            var check = _addressCheckMapper.MapCharity(charity);

            return check;
        }
    }
}

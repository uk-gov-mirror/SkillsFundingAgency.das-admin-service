using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.RoatpAssessor.Application.Extensions;
using SFA.DAS.RoatpAssessor.Application.Mappers;
using SFA.DAS.RoatpAssessor.Domain.Entities;
using SFA.DAS.RoatpAssessor.Services.ApplyApi;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Commands
{
    public class RunGatewayLegalChecksHandler : IRequestHandler<RunGatewayLegalChecksCommand>
    {
        private readonly IApplyApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IProviderDetailsToUkrlpCheckMapper _ukrlpCheckMapper;

        public RunGatewayLegalChecksHandler(IApplyApiClient applyApiClient, IQnaApiClient qnaApiClient, IProviderDetailsToUkrlpCheckMapper ukrlpCheckMapper)
        {
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
            _ukrlpCheckMapper = ukrlpCheckMapper;
        }

        public async Task<Unit> Handle(RunGatewayLegalChecksCommand request, CancellationToken cancellationToken)
        {
            var review = await _applyApiClient.GetApplicationReviewAsync(request.ApplicationId);

            if (!review.GatewayReviewIsInProgress)
            {
                throw new Exception($"Review '{request.ApplicationId}' is not in correct state to run Gateway legal checks");
            }

            var applicationData = new RoatpAssessorApplicationData(await _qnaApiClient.GetApplicationData(review.ApplicationId));

            var ukrlpCheck = await DoUkrlpCheckAsync(applicationData.UKPRN);

            string companyNumber = string.IsNullOrEmpty(applicationData.UKRLP_Verification_CompanyNumber) ?
                ukrlpCheck.CompanyNumber : applicationData.UKRLP_Verification_CompanyNumber;

            string charityRegNumber = string.IsNullOrEmpty(applicationData.UKRLP_Verification_CharityRegNumber) ? 
                ukrlpCheck.CharityRegNumber : applicationData.UKRLP_Verification_CharityRegNumber;

            var companyHouseCheckTask = TryCompaniesHouseCheckAsync(companyNumber);
            var charityCommisionTask = TryCharityCommissionCheckAsync(charityRegNumber);

            await Task.WhenAll(companyHouseCheckTask, charityCommisionTask);

            //todo update the review with results
            return Unit.Value;
        }

        private async Task<UkrlpCheck> DoUkrlpCheckAsync(string ukprn)
        {
            var response = await _applyApiClient.UkrlpLookup(ukprn);

            if (!response.Success || !response.Results.Any())
                return null;

            var providerDetails = response.Results.FirstOrDefault();

            var check = _ukrlpCheckMapper.Map(providerDetails);

            return check;
        }

        private async Task<CompaniesHouseCheck> TryCompaniesHouseCheckAsync(string companyNumber)
        {
            if (string.IsNullOrEmpty(companyNumber))
                return null;

            return null;
        }

        private async Task<CharityCommissionCheck> TryCharityCommissionCheckAsync(string charityRegNumber)
        {
            if (string.IsNullOrEmpty(charityRegNumber))
                return null;

            return null;
        }
    }
}

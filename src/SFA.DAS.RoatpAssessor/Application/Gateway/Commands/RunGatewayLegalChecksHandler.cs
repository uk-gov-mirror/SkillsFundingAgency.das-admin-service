﻿using MediatR;
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
    public class RunGatewayLegalChecksHandler : IRequestHandler<RunGatewayLegalChecksCommand>
    {
        private readonly IApplyApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly ILegalCheckMapper _legalCheckMapper;
        private readonly ITimeProvider _timeProvider;

        public RunGatewayLegalChecksHandler(
            IApplyApiClient applyApiClient, 
            IQnaApiClient qnaApiClient, 
            ILegalCheckMapper legalCheckMapper,
            ITimeProvider timeProvider)
        {
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
            _legalCheckMapper = legalCheckMapper;
            _timeProvider = timeProvider;
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

            var legalChecks = new LegalChecks
            {
                CheckedAt = _timeProvider.UtcNow,
                Ukrlp = ukrlpCheck,
                CompaniesHouse = companyHouseCheckTask.Result,
                CharityCommission = charityCommisionTask.Result
            };

            await _applyApiClient.UpdateApplicationReviewLegalChecks(review.ApplicationId, legalChecks);

            return Unit.Value;
        }

        private async Task<UkrlpLegalCheck> DoUkrlpCheckAsync(string ukprn)
        {
            var response = await _applyApiClient.UkrlpLookup(ukprn);

            if (!response.Success || !response.Results.Any())
                return null;

            var providerDetails = response.Results.FirstOrDefault();

            var check = _legalCheckMapper.MapUkrlp(providerDetails);

            return check;
        }

        private async Task<CompaniesHouseLegalCheck> TryCompaniesHouseCheckAsync(string companiesHouseNumber)
        {
            if (string.IsNullOrEmpty(companiesHouseNumber))
                return null;

            var company = await _applyApiClient.GetCompanyDetails(companiesHouseNumber);

            if (company == null)
                return null;

            var check = _legalCheckMapper.MapCompaniesHouse(company);

            return check;
        }

        private async Task<CharityCommissionLegalCheck> TryCharityCommissionCheckAsync(string charityRegNumber)
        {
            if (!int.TryParse(charityRegNumber, out var charityNo))
                return null;

            var charity = await _applyApiClient.GetCharityDetails(charityNo);

            if (charity == null)
                return null;

            var check = _legalCheckMapper.MapCharity(charity);

            return check;
        }
    }
}

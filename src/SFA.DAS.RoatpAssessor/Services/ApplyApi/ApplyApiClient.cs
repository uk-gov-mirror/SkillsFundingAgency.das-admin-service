using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.RoatpAssessor.Domain.Entities;
using SFA.DAS.RoatpAssessor.Services.ApplyApi.DTOs.Charity;
using SFA.DAS.RoatpAssessor.Services.ApplyApi.DTOs.Company;
using SFA.DAS.RoatpAssessor.Services.ApplyApi.DTOs.Ukrlp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Services.ApplyApi
{
    public class ApplyApiClient : ApiClientBase, IApplyApiClient
    {
        private readonly ILogger<ApplyApiClient> _logger;

        public ApplyApiClient(string baseUri, IApplyTokenService tokenService, ILogger<ApplyApiClient> logger)
            : base(baseUri, tokenService, logger)
        {
            _logger = logger;
        }

        public Task<List<Domain.Entities.Application>> GetSubmittedApplicationsAsync()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/Applications/Status/Submitted"))
            {
                return RequestAndDeserialiseAsync<List<Domain.Entities.Application>>(request);
            }
        }

        public Task<Domain.Entities.Application> GetApplicationAsync(Guid applicationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/Application/{applicationId}"))
            {
                return RequestAndDeserialiseAsync<Domain.Entities.Application>(request);
            }
        }

        public Task<Guid> CreateApplicationReview(Guid applicationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/ApplicationReview/CreateReview/{applicationId}"))
            {
                return RequestAndDeserialiseAsync<Guid>(request);
            }
        }

        public Task<ApplicationReview> GetApplicationReviewAsync(Guid applicationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/ApplicationReview/{applicationId}"))
            {
                return RequestAndDeserialiseAsync<ApplicationReview>(request);
            }
        }

        public Task UpdateApplicationReviewGatewayReviewAsync(Guid applicationId, ApplicationReviewStatus status)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/ApplicationReview/{applicationId}/UpdateGatewayReview"))
            {
                return PostPutRequest<object>(request, new { GatewayReviewStatus = status.ToString() });
            }
        }

        public Task UpdateApplicationReviewPmoReviewAsync(Guid applicationId, ApplicationReviewStatus status)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/ApplicationReview/{applicationId}/UpdatePmoReview"))
            {
                return PostPutRequest<object>(request, new { PmoReviewStatus = status.ToString() });
            }
        }

        public Task UpdateApplicationReviewAssessorReviewAsync(Guid applicationId, AssessorReviewNo reviewNo, ApplicationReviewStatus status)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/ApplicationReview/{applicationId}/UpdateAssessorReview"))
            {
                return PostPutRequest<object>(request, new { reviewNo, AssessorReviewStatus = status.ToString() });
            }
        }

        public Task<List<ApplicationReview>> GetActiveApplicationReviewsAsync()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/ApplicationReviews/Active"))
            {
                return RequestAndDeserialiseAsync<List<Domain.Entities.ApplicationReview>>(request);
            }
        }

        public Task UpdateAssessorComments(Guid applicationId, AssessorReviewNo reviewNo, Guid sectionId, string pageId, string comment)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/ApplicationReview/{applicationId}/UpdateAssessorComments"))
            {
                return PostPutRequest<object>(request, new { reviewNo, sectionId, pageId, comment });
            }
        }

        public Task UpdateApplicationReviewAssessorModerationAsync(Guid applicationId, ApplicationReviewStatus status)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/ApplicationReview/{applicationId}/UpdateAssessorModeration"))
            {
                return PostPutRequest<object>(request, new { AssessorModerationStatus = status.ToString() });
            }
        }

        public Task<UkrlpLookupResponse> UkrlpLookup(string ukprn)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/ukrlp-lookup?ukprn={ukprn}"))
            {
                return RequestAndDeserialiseAsync<UkrlpLookupResponse>(request);
            }
        }

        public Task<Company> GetCompanyDetails(string companiesHouseNumber)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/companies-house-lookup?companyNumber={companiesHouseNumber}"))
            {
                return RequestAndDeserialiseAsync<Company>(request);
            }
        }

        public Task<Charity> GetCharityDetails(int charityNumber)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/charity-commission-lookup?charityNumber={charityNumber}"))
            {
                return RequestAndDeserialiseAsync<Charity>(request);
            }
        }

        public Task UpdateApplicationReviewLegalChecks(Guid applicationId, LegalChecks legalChecks)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/ApplicationReview/{applicationId}/UpdateLegalChecks"))
            {
                return PostPutRequest<object>(request, legalChecks);
            }
        }

        public Task UpdateApplicationReviewAddressChecks(Guid applicationId, AddressChecks addressChecks)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/ApplicationReview/{applicationId}/UpdateAddressChecks"))
            {
                return PostPutRequest<object>(request, addressChecks);
            }
        }
    }
}

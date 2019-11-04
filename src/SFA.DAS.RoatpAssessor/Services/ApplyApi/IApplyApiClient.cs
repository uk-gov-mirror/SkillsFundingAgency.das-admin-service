using SFA.DAS.RoatpAssessor.Domain.Entities;
using SFA.DAS.RoatpAssessor.Services.ApplyApi.DTOs.Charity;
using SFA.DAS.RoatpAssessor.Services.ApplyApi.DTOs.Company;
using SFA.DAS.RoatpAssessor.Services.ApplyApi.DTOs.Ukrlp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Services.ApplyApi
{
    public interface IApplyApiClient
    {
        Task<List<Domain.Entities.Application>> GetSubmittedApplicationsAsync();
        Task<Domain.Entities.Application> GetApplicationAsync(Guid applicationId);
        Task<Guid> CreateApplicationReview(Guid applicationId);
        Task<ApplicationReview> GetApplicationReviewAsync(Guid applicationId);
        Task UpdateApplicationReviewGatewayReviewAsync(Guid applicationId, ApplicationReviewStatus status);
        Task UpdateApplicationReviewPmoReviewAsync(Guid applicationId, ApplicationReviewStatus status);
        Task UpdateApplicationReviewAssessorReviewAsync(Guid applicationId, AssessorReviewNo reviewNo, ApplicationReviewStatus status);
        Task<List<ApplicationReview>> GetActiveApplicationReviewsAsync();
        Task UpdateAssessorComments(Guid applicationId, AssessorReviewNo reviewNo, Guid sectionId, string pageId, string comment);
        Task UpdateApplicationReviewAssessorModerationAsync(Guid applicationId, ApplicationReviewStatus status);
        Task<UkrlpLookupResponse> UkrlpLookup(string ukprn);
        Task<Company> GetCompanyDetails(string companiesHouseNumber);
        Task<Charity> GetCharityDetails(int charityNumber);
        Task UpdateApplicationReviewLegalChecks(Guid applicationId, LegalChecks legalChecks);
        Task UpdateApplicationReviewAddressChecks(Guid applicationId, AddressChecks addressChecks);
    }
}
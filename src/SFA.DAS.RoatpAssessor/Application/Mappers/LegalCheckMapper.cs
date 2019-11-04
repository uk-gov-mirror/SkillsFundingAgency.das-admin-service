using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
using SFA.DAS.RoatpAssessor.Domain.Entities;
using Company = SFA.DAS.RoatpAssessor.Services.ApplyApi.DTOs.Company;
using Charity = SFA.DAS.RoatpAssessor.Services.ApplyApi.DTOs.Charity;
using System.Linq;

namespace SFA.DAS.RoatpAssessor.Application.Mappers
{
    public class LegalCheckMapper : ILegalCheckMapper
    {
        public UkrlpLegalCheck MapUkrlp(ProviderDetails providerDetails)
        {
            if (providerDetails == null)
                return null;

            var check = new UkrlpLegalCheck
            {
                Status = providerDetails.ProviderStatus,
                TradingName = providerDetails.ProviderAliases?[0].Alias,
                LegalName = providerDetails.ProviderName
            };

            SetUkrlpCompaniesHouseVerification(providerDetails, check);
            SetUkrlpCharityVerification(providerDetails, check);

            return check;
        }

        private void SetUkrlpCompaniesHouseVerification(ProviderDetails providerDetails, UkrlpLegalCheck check)
        {
            var companiesHouseVerification = providerDetails.VerificationDetails?.FirstOrDefault(v =>
                v.VerificationAuthority == VerificationAuthorities.CompaniesHouseAuthority);

            check.CompanyNumber = (companiesHouseVerification == null) ? null : companiesHouseVerification.VerificationId;
        }

        private void SetUkrlpCharityVerification(ProviderDetails providerDetails, UkrlpLegalCheck check)
        {
            var charityCommissionVerification = providerDetails.VerificationDetails?.FirstOrDefault(v =>
                v.VerificationAuthority == VerificationAuthorities.CharityCommissionAuthority);

            check.CharityRegNumber = (charityCommissionVerification == null) ? null : charityCommissionVerification.VerificationId;
        }

        public CompaniesHouseLegalCheck MapCompaniesHouse(Company.Company company)
        {
            if(company == null)
                return null;

            var check = new CompaniesHouseLegalCheck
            {
                LegalName = company.CompanyName,
                CompanyNumber = company.CompanyNumber,
                Status = company.Status
            };

            return check;
        }

        public CharityCommissionLegalCheck MapCharity(Charity.Charity charity)
        {
            if (charity == null)
                return null;

            var check = new CharityCommissionLegalCheck
            {
                LegalName = charity.Name,
                CharityRegNumber = charity.CharityNumber,
                Status = charity.Status
            };

            return check;
        }
    }
}

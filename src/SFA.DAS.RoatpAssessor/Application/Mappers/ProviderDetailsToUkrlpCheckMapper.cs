using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
using SFA.DAS.RoatpAssessor.Domain.Entities;
using System.Linq;

namespace SFA.DAS.RoatpAssessor.Application.Mappers
{
    public class ProviderDetailsToUkrlpCheckMapper : IProviderDetailsToUkrlpCheckMapper
    {
        public UkrlpCheck Map(ProviderDetails providerDetails)
        {
            var check = new UkrlpCheck
            {
                LegalName = providerDetails.ProviderName,
                Website = providerDetails.PrimaryContactDetails?.ContactWebsiteAddress,
                LegalAddressLine1 = providerDetails.PrimaryContactDetails?.ContactAddress?.Address1,
                LegalAddressLine2 = providerDetails.PrimaryContactDetails?.ContactAddress?.Address2,
                LegalAddressLine3 = providerDetails.PrimaryContactDetails?.ContactAddress?.Address3,
                LegalAddressLine4 = providerDetails.PrimaryContactDetails?.ContactAddress?.Address4,
                LegalAddressTown = providerDetails.PrimaryContactDetails?.ContactAddress?.Town,
                LegalAddressPostcode = providerDetails.PrimaryContactDetails?.ContactAddress?.PostCode,
                TradingName = providerDetails?.ProviderAliases?[0].Alias
            };

            SetCompaniesHouseVerification(providerDetails, check);
            SetCharityVerification(providerDetails, check);
            SetSoleTraderPartnershipVerification(providerDetails, check);

            return check;
        }

        private void SetCompaniesHouseVerification(ProviderDetails providerDetails, UkrlpCheck check)
        {
            var companiesHouseVerification = providerDetails.VerificationDetails?.FirstOrDefault(v =>
                v.VerificationAuthority == VerificationAuthorities.CompaniesHouseAuthority);

            check.HasCompaniesHouseVerification = (companiesHouseVerification == null);
            check.CompanyNumber = (companiesHouseVerification == null) ? string.Empty : companiesHouseVerification.VerificationId;
        }

        private void SetCharityVerification(ProviderDetails providerDetails, UkrlpCheck check)
        {
            var charityCommissionVerification = providerDetails.VerificationDetails?.FirstOrDefault(v =>
                v.VerificationAuthority == VerificationAuthorities.CharityCommissionAuthority);

            check.HasCharityCommissionVerification = (charityCommissionVerification == null);
            check.CharityRegNumber = (charityCommissionVerification == null) ? string.Empty : charityCommissionVerification.VerificationId;
        }

        private void SetSoleTraderPartnershipVerification(ProviderDetails providerDetails, UkrlpCheck check)
        {
            check.HasSoleTraderPartnershipVerification = providerDetails.VerificationDetails?.Any(v =>
                v.VerificationAuthority == VerificationAuthorities.SoleTraderPartnershipAuthority) != null;
        }
    }
}

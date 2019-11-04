using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
using SFA.DAS.RoatpAssessor.Domain.Entities;
using Company = SFA.DAS.RoatpAssessor.Services.ApplyApi.DTOs.Company;
using Charity = SFA.DAS.RoatpAssessor.Services.ApplyApi.DTOs.Charity;
using System.Linq;
using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Application.Mappers
{
    public class AddressCheckMapper : IAddressCheckMapper
    {
        public AddressCheck MapUkrlp(ProviderDetails provider)
        {
            if(provider == null)
                return null;

            var addressLines = new List<string>
            {
                provider.PrimaryContactDetails?.ContactAddress?.Address1,
                provider.PrimaryContactDetails?.ContactAddress?.Address2,
                provider.PrimaryContactDetails?.ContactAddress?.Address3,
                provider.PrimaryContactDetails?.ContactAddress?.Address4,
                provider.PrimaryContactDetails?.ContactAddress?.Town
            }.Where(a => !string.IsNullOrEmpty(a)).ToList();


            var check = new AddressCheck
            {
                AddressLines = addressLines,
                Postcode = provider.PrimaryContactDetails?.ContactAddress?.PostCode
            };

            return check;
        }

        public AddressCheck MapCompaniesHouse(Company.Company company)
        {
            if (company == null)
                return null;

            var addressLines = new List<string>
            {
                company.RegisteredOfficeAddress?.AddressLine1,
                company.RegisteredOfficeAddress?.AddressLine2,
                company.RegisteredOfficeAddress?.City,
                company.RegisteredOfficeAddress?.County,
                company.RegisteredOfficeAddress?.Country
            }.Where(a => !string.IsNullOrEmpty(a)).ToList();

            var check = new AddressCheck
            {
                AddressLines = addressLines,
                Postcode = company.RegisteredOfficeAddress?.PostalCode
            };

            return check;
        }

        public AddressCheck MapCharity(Charity.Charity charity)
        {
            if (charity == null)
                return null;

            var addressLines = new List<string>
            {
                charity.RegisteredOfficeAddress?.AddressLine1,
                charity.RegisteredOfficeAddress?.AddressLine2,
                charity.RegisteredOfficeAddress?.City,
                charity.RegisteredOfficeAddress?.County,
                charity.RegisteredOfficeAddress?.Country
            }.Where(a => !string.IsNullOrEmpty(a)).ToList();

            var check = new AddressCheck
            {
                AddressLines = addressLines,
                Postcode = charity.RegisteredOfficeAddress?.PostalCode
            };

            return check;
        }
    }
}

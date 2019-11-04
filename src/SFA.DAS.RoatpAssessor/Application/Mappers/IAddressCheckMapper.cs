using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
using SFA.DAS.RoatpAssessor.Domain.Entities;
using SFA.DAS.RoatpAssessor.Services.ApplyApi.DTOs.Charity;
using SFA.DAS.RoatpAssessor.Services.ApplyApi.DTOs.Company;
using System;

namespace SFA.DAS.RoatpAssessor.Application.Mappers
{
    public interface IAddressCheckMapper
    {
        AddressCheck MapUkrlp(ProviderDetails providerDetails);
        AddressCheck MapCompaniesHouse(Company company);
        AddressCheck MapCharity(Charity charity);
    }
}
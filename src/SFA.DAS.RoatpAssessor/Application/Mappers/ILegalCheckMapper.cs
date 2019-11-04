using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
using SFA.DAS.RoatpAssessor.Domain.Entities;
using SFA.DAS.RoatpAssessor.Services.ApplyApi.DTOs.Charity;
using SFA.DAS.RoatpAssessor.Services.ApplyApi.DTOs.Company;
using System;

namespace SFA.DAS.RoatpAssessor.Application.Mappers
{
    public interface ILegalCheckMapper
    {
        UkrlpLegalCheck MapUkrlp(ProviderDetails providerDetails);
        CompaniesHouseLegalCheck MapCompaniesHouse(Company company);
        CharityCommissionLegalCheck MapCharity(Charity charity);
    }
}
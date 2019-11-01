using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
using SFA.DAS.RoatpAssessor.Domain.Entities;

namespace SFA.DAS.RoatpAssessor.Application.Mappers
{
    public interface IProviderDetailsToUkrlpCheckMapper
    {
        UkrlpCheck Map(ProviderDetails providerDetails);
    }
}
using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Services.ApplyApi.DTOs.Ukrlp
{
    public class UkrlpLookupResponse
    {
        public bool Success { get; set; }
        public List<ProviderDetails> Results { get; set; }
    }
}

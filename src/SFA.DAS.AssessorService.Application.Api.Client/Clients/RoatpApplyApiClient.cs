using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using RoatpAssessorTypes = SFA.DAS.AssessorService.Api.Types.Models.Roatp.Assessor;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class RoatpApplyApiClient : IRoatpApplyApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<RoatpApplyApiClient> _logger;
        private readonly ITokenService _tokenService;

        public RoatpApplyApiClient(HttpClient client, ILogger<RoatpApplyApiClient> logger, ITokenService tokenService)
        {
            _client = client;
            _logger = logger;
            _tokenService = tokenService;
        }

        public RoatpApplyApiClient(string baseUri, ILogger<RoatpApplyApiClient> logger, ITokenService tokenService)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUri) };
            _logger = logger;
            _tokenService = tokenService;
        }


        public async Task<List<RoatpAssessorTypes.Application>> GetSubmittedApplications()
        {
            return await Get<List<RoatpAssessorTypes.Application>>($"/roatp-assessor/applications/submitted");
        }

        public async Task<RoatpAssessorTypes.GatewayCounts> GetGatewayCounts()
        {
            return await Get<RoatpAssessorTypes.GatewayCounts>($"/roatp-assessor/gateway/counts");
        }
               
        private async Task<T> Get<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            using (var response = await _client.GetAsync(new Uri(uri, UriKind.Relative)))
            {
                return await response.Content.ReadAsAsync<T>();
            }
        }
    }
}

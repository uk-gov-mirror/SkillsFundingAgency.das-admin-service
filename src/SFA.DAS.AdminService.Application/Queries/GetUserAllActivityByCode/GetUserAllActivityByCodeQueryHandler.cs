using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Admin;

namespace SFA.DAS.AdminService.Application.Queries.GetUserAllActivityByCode
{
    public class GetUserAllActivityByCodeQueryHandler : IRequestHandler<GetUserAllActivityByCodeQuery, GetUserAllActivityByCodeQueryResult>
    {
        private readonly IAdminOuterApi _adminOuterApi;
        private readonly ILogger<GetUserAllActivityByCodeQueryHandler> _logger;

        public GetUserAllActivityByCodeQueryHandler(IAdminOuterApi adminOuterApi, ILogger<GetUserAllActivityByCodeQueryHandler> logger)
        {
            _adminOuterApi = adminOuterApi;
            _logger = logger;
        }

        public async Task<GetUserAllActivityByCodeQueryResult> Handle(GetUserAllActivityByCodeQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _adminOuterApi.GetUserAllActivity(query.Code);
                return (GetUserAllActivityByCodeQueryResult)response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Admin outer API for all-activity code {Code}", query.Code);
                throw;
            }
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Admin;

namespace SFA.DAS.AdminService.Application.Queries.GetUserActionByCode

{
    public class GetUserActionByCodeQueryHandler : IRequestHandler<GetUserActionByCodeQuery, GetUserActionByCodeQueryResult>
    {
        private readonly IAdminOuterApi _adminOuterApi;
        private readonly ILogger<GetUserActionByCodeQueryHandler> _logger;

        public GetUserActionByCodeQueryHandler(IAdminOuterApi adminOuterApi, ILogger<GetUserActionByCodeQueryHandler> logger)
        {
            _adminOuterApi = adminOuterApi;
            _logger = logger;
        }

        public async Task<GetUserActionByCodeQueryResult> Handle(GetUserActionByCodeQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _adminOuterApi.GetUserActionByCode(query.Code);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Admin outer API for user action code {Code}", query.Code);
                throw;
            }
        }
    }
}

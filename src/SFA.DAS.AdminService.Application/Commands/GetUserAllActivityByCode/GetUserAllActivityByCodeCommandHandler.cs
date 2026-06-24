using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Admin;

namespace SFA.DAS.AdminService.Application.Commands.GetUserAllActivityByCode
{
    public class GetUserAllActivityByCodeCommandHandler : IRequestHandler<GetUserAllActivityByCodeCommand, GetUserAllActivityByCodeCommandResult>
    {
        private readonly IAdminOuterApi _adminOuterApi;
        private readonly ILogger<GetUserAllActivityByCodeCommandHandler> _logger;

        public GetUserAllActivityByCodeCommandHandler(IAdminOuterApi adminOuterApi, ILogger<GetUserAllActivityByCodeCommandHandler> logger)
        {
            _adminOuterApi = adminOuterApi;
            _logger = logger;
        }

        public async Task<GetUserAllActivityByCodeCommandResult> Handle(GetUserAllActivityByCodeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _adminOuterApi.GetUserAllActivity(request.Code);
                return (GetUserAllActivityByCodeCommandResult)response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Admin outer API for all-activity code {Code}", request.Code);
                throw;
            }
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Admin;

namespace SFA.DAS.AdminService.Application.Commands.CheckUserActionByCode
{
    public class CheckUserActionByCodeCommandHandler : IRequestHandler<CheckUserActionByCodeCommand, CheckUserActionByCodeCommandResult>
    {
        private readonly IAdminOuterApi _adminOuterApi;
        private readonly ILogger<CheckUserActionByCodeCommandHandler> _logger;

        public CheckUserActionByCodeCommandHandler(IAdminOuterApi adminOuterApi, ILogger<CheckUserActionByCodeCommandHandler> logger)
        {
            _adminOuterApi = adminOuterApi;
            _logger = logger;
        }

        public async Task<CheckUserActionByCodeCommandResult> Handle(CheckUserActionByCodeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _adminOuterApi.CheckUserActionByCode(request.Code, request);
                return response;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling Digital Certificates API for code {Code}", request.Code);
                throw;
            }
        }
    }
}

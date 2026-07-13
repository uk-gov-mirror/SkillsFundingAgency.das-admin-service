using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Admin;

namespace SFA.DAS.AdminService.Application.Commands.UnlockUser
{
    public class UnlockUserCommandHandler : IRequestHandler<UnlockUserCommand, Unit>
    {
        private readonly IAdminOuterApi _adminOuterApi;
        private readonly ILogger<UnlockUserCommandHandler> _logger;

        public UnlockUserCommandHandler(IAdminOuterApi adminOuterApi, ILogger<UnlockUserCommandHandler> logger)
        {
            _adminOuterApi = adminOuterApi;
            _logger = logger;
        }

        public async Task<Unit> Handle(UnlockUserCommand command, CancellationToken cancellationToken)
        {
            try
            {
                await _adminOuterApi.UnlockUser(command.UserId, command);
                return Unit.Value;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling Admin outer API to unlock user {UserId}", command.UserId);
                throw;
            }
        }
    }
}

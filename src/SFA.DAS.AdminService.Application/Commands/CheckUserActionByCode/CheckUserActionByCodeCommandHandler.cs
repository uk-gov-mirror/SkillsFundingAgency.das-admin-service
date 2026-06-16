using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Infrastructure.ApiClients.DigitalCertificates;

namespace SFA.DAS.AdminService.Application.Commands.CheckUserActionByCode
{
    public class CheckUserActionByCodeCommandHandler : IRequestHandler<CheckUserActionByCodeCommand, CheckUserActionByCodeCommandResult>
    {
        private readonly IDigitalCertificatesOuterApi _digitalCertificatesOuterApi;
        private readonly ILogger<CheckUserActionByCodeCommandHandler> _logger;

        public CheckUserActionByCodeCommandHandler(IDigitalCertificatesOuterApi digitalCertificatesOuterApi, ILogger<CheckUserActionByCodeCommandHandler> logger)
        {
            _digitalCertificatesOuterApi = digitalCertificatesOuterApi;
            _logger = logger;
        }

        public async Task<CheckUserActionByCodeCommandResult> Handle(CheckUserActionByCodeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _digitalCertificatesOuterApi.CheckUserActionByCode(request.Code, request);
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

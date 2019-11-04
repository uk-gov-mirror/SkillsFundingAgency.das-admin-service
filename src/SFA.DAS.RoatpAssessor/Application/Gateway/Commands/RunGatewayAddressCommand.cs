using MediatR;
using System;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Commands
{
    public class RunGatewayAddressChecksCommand : IRequest
    {
        public Guid ApplicationId { get; }

        public RunGatewayAddressChecksCommand(Guid applicationId)
        {
            ApplicationId = applicationId;
        }
    }
}

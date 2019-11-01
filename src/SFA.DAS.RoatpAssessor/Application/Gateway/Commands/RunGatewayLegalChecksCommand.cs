using MediatR;
using System;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Commands
{
    public class RunGatewayLegalChecksCommand : IRequest
    {
        public Guid ApplicationId { get; }

        public RunGatewayLegalChecksCommand(Guid applicationId)
        {
            ApplicationId = applicationId;
        }
    }
}

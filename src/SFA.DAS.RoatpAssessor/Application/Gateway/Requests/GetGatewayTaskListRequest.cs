using MediatR;
using System;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Requests
{
    public class GetGatewayTaskListRequest : IRequest<GetGatewayTaskListResponse>
    {
        public Guid ApplicationId { get;}

        public GetGatewayTaskListRequest(Guid applicationId)
        {
            ApplicationId = applicationId;
        }
    }
}

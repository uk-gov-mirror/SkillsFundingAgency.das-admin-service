using MediatR;

namespace SFA.DAS.AdminService.Application.Queries.GetUserAllActivityByCode
{
    public class GetUserAllActivityByCodeQuery : IRequest<GetUserAllActivityByCodeQueryResult>
    {
        public required string Code { get; set; }
    }
}

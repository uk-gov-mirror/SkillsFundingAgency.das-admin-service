using MediatR;

namespace SFA.DAS.AdminService.Application.Queries.GetUserActionByCode
{
    public class GetUserActionByCodeQuery : IRequest<GetUserActionByCodeQueryResult>
    {
        public string Code { get; set; } = string.Empty;
    }
}

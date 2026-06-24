using MediatR;

namespace SFA.DAS.AdminService.Application.Commands.GetUserAllActivityByCode
{
    public class GetUserAllActivityByCodeCommand : IRequest<GetUserAllActivityByCodeCommandResult>
    {
        public required string Code { get; set; }
    }
}

using MediatR;

namespace SFA.DAS.AdminService.Application.Commands.CheckUserActionByCode
{
    public class CheckUserActionByCodeCommand : IRequest<CheckUserActionByCodeCommandResult>
    {
        public string Code { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;

        public static implicit operator Infrastructure.Api.Requests.CheckUserActionByCodeRequest(CheckUserActionByCodeCommand command)
        {
            return new Infrastructure.Api.Requests.CheckUserActionByCodeRequest
            {
                Username = command.Username
            };
        }
    }
}

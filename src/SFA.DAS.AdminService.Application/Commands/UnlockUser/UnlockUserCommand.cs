using System;
using MediatR;
using SFA.DAS.AdminService.Infrastructure.Api.Requests;

namespace SFA.DAS.AdminService.Application.Commands.UnlockUser
{
    public class UnlockUserCommand : IRequest<Unit>
    {
        public Guid UserId { get; set; }
        public required string Username { get; set; }
        public long UserActionId { get; set; }

        public static implicit operator UnlockUserRequest(UnlockUserCommand command)
        {
            return new UnlockUserRequest
            {
                Username = command.Username,
                UserActionId = command.UserActionId
            };
        }
    }
}

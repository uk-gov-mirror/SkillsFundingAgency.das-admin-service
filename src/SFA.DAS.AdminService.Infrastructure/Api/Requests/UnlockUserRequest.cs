namespace SFA.DAS.AdminService.Infrastructure.Api.Requests
{
    public class UnlockUserRequest
    {
        public required string Username { get; set; }
        public long UserActionId { get; set; }
    }
}

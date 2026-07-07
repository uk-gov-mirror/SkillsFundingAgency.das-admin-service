namespace SFA.DAS.AdminService.Common.Models
{
    public enum ActionType
    {
        Reprint = 1,
        Help = 2,
        Contact = 3,
        NotMatched = 4,
        NotFound = 5,
    }

    public enum CertificateType
    {
        Unknown,
        Standard,
        Framework
    }

    public enum UserActionStatus
    {
        New,
        Viewed
    }

    public enum AdminActionType
    {
        Viewed,
        Unlocked
    }
}

using System;
using System.Collections.Generic;
using SFA.DAS.AdminService.Common.Models;

namespace SFA.DAS.AdminService.Web.ViewModels.Search
{
    public class UserNotMatchedViewModel
    {
        public required string ReferenceNumber { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public bool IsUserLocked { get; set; }
        public List<UserAccessHistoryItem> History { get; set; } = new List<UserAccessHistoryItem>();
    }

    public class UserAccessHistoryItem
    {
        public string FormattedActionTime { get; set; } = string.Empty;
        public required ActionType ActionType { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public bool IsUnlocked { get; set; }
        public string UnlockedBy { get; set; } = string.Empty;
        public string FormattedUnlockedTime { get; set; } = string.Empty;
        public List<UserAttempt> Attempts { get; set; } = new List<UserAttempt>();
        public string TagClass { get; set; } = string.Empty;
        public string TagText { get; set; } = string.Empty;
    }

    public class UserAttempt
    {
        public required string FormattedEventTime { get; set; }
        public required string Uln { get; set; }
        public required string CourseName { get; set; }
        public required string DateAwarded { get; set; }
        public required string ProviderName { get; set; }
    }
}

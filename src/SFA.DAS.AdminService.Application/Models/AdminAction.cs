using System;

namespace SFA.DAS.AdminService.Application.Models
{
    public class AdminAction
    {
        public string Username { get; set; }
        public DateTime ActionTime { get; set; }
        public string Action { get; set; }
    }
}

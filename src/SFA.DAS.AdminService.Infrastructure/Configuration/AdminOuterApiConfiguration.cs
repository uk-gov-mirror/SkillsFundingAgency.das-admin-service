using System.Diagnostics.CodeAnalysis;
using SFA.DAS.Http.Configuration;

namespace SFA.DAS.AdminService.Infrastructure.Configuration
{
    [ExcludeFromCodeCoverage]
    public class AdminOuterApiConfiguration : IApimClientConfiguration
    {
        public required string ApiBaseUrl { get; set; }
        public required string SubscriptionKey { get; set; }
        public required string ApiVersion { get; set; }
    }
}

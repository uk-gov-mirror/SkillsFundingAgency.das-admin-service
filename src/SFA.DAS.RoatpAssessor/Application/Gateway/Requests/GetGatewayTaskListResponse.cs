using System;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Requests
{
    public class GetGatewayTaskListResponse
    {
        public Guid ApplicationId { get; set; }
        public string Ukprn { get; set; }
        public string Name { get; set; }
        public string ProviderRoute { get; set; }
        public string ApplicationRef { get; set; }
        public DateTime SubmittedAt { get; set; }
        public bool CanDoLegalChecks { get; set; }
        public bool CanDoAddressChecks { get; set; }
    }
}

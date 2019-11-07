using System;

namespace SFA.DAS.RoatpAssessor.Application.Models
{
    public class ReviewSummary
    {
        public Guid ApplicationId { get; set; }
        public string Name { get; set; }
        public string Ukprn { get; set; }
        public string ApplicationRef { get; set; }
        public string ProviderRoute { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}

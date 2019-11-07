namespace SFA.DAS.RoatpAssessor.Application
{
    public static class RoatpWorkflowSectionIds
    {
        public static class YourOrganisation
        {
            public static int ProviderRoute = 1;
            public static int WhatYouWillNeed = 2;
            public static int OrganisationDetails = 3;
            public static int WhosInControl = 4;
            public static int DescribeYourOrganisation = 5;
            public static int ExperienceAndAccreditations = 6;
        }

        public static class FinancialEvidence
        {
            public static int WhatYouWillNeed = 1;
            public static int YourOrganisationsFinancialEvidence = 2;
        }

        public static class CriminalComplianceChecks
        {
            public static int WhatYouWillNeed = 1;
            public static int ChecksOnYourOrganisation = 2;
            public static int CheckOnWhosInControl = 3;
        }
    }
}

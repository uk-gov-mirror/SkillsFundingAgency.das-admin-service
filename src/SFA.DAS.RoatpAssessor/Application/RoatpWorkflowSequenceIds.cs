using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Application
{
    public static class RoatpWorkflowSequenceIds
    {
        public static int Preamble = 0;
        public static int YourOrganisation = 1;
        public static int FinancialEvidence = 2;
        public static int CriminalComplianceChecks = 3;
        public static int ApprenticeshipWelfare = 4;
        public static int ReadinessToEngage = 5;
        public static int PeopleAndPlanning = 6;
        public static int LeadersAndManagers = 7;
        public static int ConditionsOfAcceptance = 99;

        public static IEnumerable<int> GatewaySequences => new int[] {
            Preamble,
            YourOrganisation,
            CriminalComplianceChecks,
            ConditionsOfAcceptance
        };
    }
}

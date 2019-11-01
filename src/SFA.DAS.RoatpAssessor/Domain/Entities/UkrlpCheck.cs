namespace SFA.DAS.RoatpAssessor.Domain.Entities
{
    public class UkrlpCheck
    {
        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public string LegalAddressLine1 { get; set; }
        public string LegalAddressLine2 { get; set; }
        public string LegalAddressLine3 { get; set; }
        public string LegalAddressLine4 { get; set; }
        public string LegalAddressTown { get; set; }
        public string LegalAddressPostcode { get; set; }
        public string CompanyNumber { get; set; }
        public string CharityRegNumber { get; set; }
        public string Website { get; set; }
        public bool HasCompaniesHouseVerification { get; set; }
        public bool HasCharityCommissionVerification { get; set; }
        public bool HasSoleTraderPartnershipVerification { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Domain.Entities
{
    public class LegalChecks
    {
        public DateTime CheckedAt { get; set; }
        public UkrlpLegalCheck Ukrlp { get; set; }
        public CompaniesHouseLegalCheck CompaniesHouse { get; set; }
        public CharityCommissionLegalCheck CharityCommission { get; set; }
    }

    public class UkrlpLegalCheck
    {
        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public string CompanyNumber { get; set; }
        public string CharityRegNumber { get; set; }
        public string Status { get; set; }
    }

    public class CompaniesHouseLegalCheck
    {
        public string LegalName { get; set; }
        public string CompanyNumber { get; set; }
        public string Status { get; set; }
    }

    public class CharityCommissionLegalCheck
    {
        public string LegalName { get; set; }
        public string CharityRegNumber { get; set; }
        public string Status { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Domain.Entities
{
    public class AddressChecks
    {
        public DateTime CheckedAt { get; set; }
        public AddressCheck Ukrlp { get; set; }
        public AddressCheck CompaniesHouse { get; set; }
        public AddressCheck CharityCommission { get; set; }
    }

    public class AddressCheck
    {
        public IEnumerable<string> AddressLines { get; set; }
        public string Postcode { get; set; }
    }
}

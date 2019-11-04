using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Services.ApplyApi.DTOs.Charity
{
    public class Charity
    {
        public string CharityNumber { get; set; }
        public string CompanyNumber { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }

        public Address RegisteredOfficeAddress { get; set; }

        public IEnumerable<string> NatureOfBusiness { get; set; }

        public DateTime? IncorporatedOn { get; set; }
        public DateTime? DissolvedOn { get; set; }

        public Accounts Accounts { get; set; }

        public IEnumerable<Trustee> Trustees { get; set; }

        public bool IsActivelyTrading
        {
            get
            {
                return Status.Equals("registered", StringComparison.InvariantCultureIgnoreCase) && DissolvedOn == null;
            }
        }

        public bool TrusteeManualEntryRequired { get; set; }
    }

    public class Address
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
    }

    public class Accounts
    {
        public DateTime? LastAccountsDate { get; set; }
    }

    public class Trustee
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

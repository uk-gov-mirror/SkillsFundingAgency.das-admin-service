﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.ApplyTypes.CharityCommission
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

        public IEnumerable<TrusteeInformation> Trustees { get; set; }

        public bool IsActivelyTrading
        {
            get
            {
                return Status.Equals("registered", StringComparison.InvariantCultureIgnoreCase) && DissolvedOn == null;
            }
        }

        public bool TrusteeManualEntryRequired { get; set; }
    }
}
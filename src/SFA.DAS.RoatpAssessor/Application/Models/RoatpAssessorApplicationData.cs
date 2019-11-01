using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Application.Extensions
{
    public class RoatpAssessorApplicationData
    {
        private const string Key_Apply_ProviderRoute = "Apply-ProviderRoute";
        private const string Key_UKPRN = "UKPRN";
        private const string Key_UKRLP_LegalName = "UKRLP-LegalName";
        private const string Key_UKRLP_Verification_CharityRegNumber = "UKRLP-Verification-CharityRegNumber";
        private const string Key_UKRLP_Verification_CompanyNumber = "UKRLP-Verification-CompanyNumber";
        

        private readonly Dictionary<string, object> _applicationData;

        public RoatpAssessorApplicationData(Dictionary<string, object> applicationData)
        {
            _applicationData = applicationData;
        }

        public ProviderRoutes Apply_ProviderRoute => Enum.Parse<ProviderRoutes>(_applicationData[Key_Apply_ProviderRoute].ToString());

        public string UKPRN => _applicationData[Key_UKPRN]?.ToString();

        public string UKRLP_LegalName => _applicationData[Key_UKRLP_LegalName]?.ToString();

        public string UKRLP_Verification_CharityRegNumber => _applicationData[Key_UKRLP_Verification_CharityRegNumber]?.ToString();

        public string UKRLP_Verification_CompanyNumber => _applicationData[Key_UKRLP_Verification_CompanyNumber]?.ToString();
    }
}

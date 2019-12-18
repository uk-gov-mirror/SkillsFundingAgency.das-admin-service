﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web
{
    public static class RouteNames
    {
        public const string RoatpDashboard_Index_Get = "RoatpDashboard_Index_Get";

        //Roatp
        public const string Roatp_Home_Index_Get = "Roatp_Home_Index_Get";
        public const string Roatp_AddRoatpOrganisation_EnterUkprn_Get = "Roatp_AddRoatpOrganisation_EnterUkprn_Get";
        public const string Roatp_DownloadRoatp_Download_Get = "Roatp_AddRoatpOrganisation_Download_Get";

        //RoatpAssesssor
        public const string RoatpAssessor_Gateway_Dashboard_Get = "RoatpAssessor_Gateway_Dashboard_Get";
        public const string RoatpAssessor_Gateway_Start_Review = "RoatpAssessor_Gateway_Start_Review";
        public const string RoatpAssessor_Assessor_Dashboard_Get = "RoatpAssessor_Assessor_Dashboard_Get";
    }
}

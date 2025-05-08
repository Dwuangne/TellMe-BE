using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Constants
{
    public static class PathConstant
    {
        public static class PathTemplate
        {
            public const string RouteEmail = "Templates/Emails";
            public const string ConfirmEmail = RouteEmail + "/Confirmed.html";
            public const string ConfirmEmailSuccess = RouteEmail + "/ConfirmedSuccess.html";
            public const string ConfirmEmailFailure = RouteEmail + "/ConfirmedFailure.html";
            public const string ForgotPassword = RouteEmail + "/ConfirmedFailure.html";
        }
        public static class PathAssets
        {
            public const string RouteLogos = "Assets/Logos";
            public const string TellMe = RouteLogos + "/TellMe.jpg";
        }
    }
}

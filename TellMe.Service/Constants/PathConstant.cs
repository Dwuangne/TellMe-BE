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
            public const string ConfirmEmail = RouteEmail + "/ConfirmedEmail.html";
            public const string ConfirmEmailSuccess = RouteEmail + "/ConfirmedEmailSuccess.html";
            public const string ConfirmEmailFailure = RouteEmail + "/ConfirmedEmailFailure.html";
        }
    }
}

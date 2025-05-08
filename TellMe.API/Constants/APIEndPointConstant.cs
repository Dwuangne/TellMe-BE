namespace TellMe.API.Constants
{
    public static class APIEndPointConstant
    {
        private const string RootEndPoint = "/api";
        private const string ApiVersion = "/v1";
        private const string ApiEndPoint = RootEndPoint + ApiVersion;

        public static class Authentication
        {
            public const string AuthenticationEndpoint = ApiEndPoint + "/authentication";
            public const string Login = "login";
            public const string Register = "register";
            public const string ConfirmEmail = "email-confirmation";
            public const string LoginGoogle = "login-google";
            public const string RefreshToken = "refresh-token";
        }

        public static class Account
        {
            public const string AccountEndpoint = ApiEndPoint + "/account";
            public const string GetAccount = "get-account";
            public const string UpdateAccount = "update-account";
            public const string ChangePassword = "change-password";
            public const string ForgotPassword = "forgot-password";
            public const string ResetPassword = "reset-password";
        }
    }
}

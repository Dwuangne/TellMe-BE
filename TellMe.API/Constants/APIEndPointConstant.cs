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
        public static class PsychologicalTest
        {
            public const string PsychologicalTestEndpoint = ApiEndPoint + "/psychological-test";
        }

        public static class UserTest
        {
            public const string UserTestEndpoint = ApiEndPoint + "/user-test";
        }

        public static class Psychologist
        {
            public const string PsychologistEndpoint = ApiEndPoint + "/psychologist";
        }
        public static class PsychologistReview
        {
            public const string PsychologistReviewEndpoint = ApiEndPoint + "/psychologist-reviews";
        }

        public static class SubscriptionPackage
        {
            public const string SubscriptionPackageEndpoint = ApiEndPoint + "/subscription-packages";
        }

        public static class Payment
        {
            public const string PaymentEndpoint = ApiEndPoint + "/payments";
        }

        public static class UserSubscription
        {
            public const string UserSubscriptionEndpoint = ApiEndPoint + "/usersubscriptions";
        }

        public static class Appointment
        {
            public const string AppointmentEndpoint = ApiEndPoint + "/appointments";
        }
        public static class CheckOut
        {
            public const string CheckOutEndpoint = ApiEndPoint + "/check-out";
            public const string CreateCheckOut = "/create";
            public const string ReturnCheckOut = "/return_url";
            public const string CancelCheckOut = "/cancel_url";
        }

        public static class Conversation
        {
            public const string ConversationEndpoint = ApiEndPoint + "/conversations";
        }

        public static class Message
        {
            public const string MessageEndpoint = ApiEndPoint + "/messages";
        }

    }
}

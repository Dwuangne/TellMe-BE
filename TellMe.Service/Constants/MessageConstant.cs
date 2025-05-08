using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Constants
{
    public static class MessageConstant
    {
        public static class Authentication
        {
            public static class Login
            {
                public const string EmailNotFound = "Email does not exist.";
                public const string AccountDisabled = "Account is disabled.";
                public const string EmailNotVerified = "Email is not verified.";
                public const string InvalidCredentials = "Invalid email or password.";
            }

            public static class Register
            {
                public const string EmailExists = "Email already exists.";
                public const string RegistrationFailed = "Failed to register new account.";
            }

            public static class RefreshToken
            {
                public const string InvalidToken = "Invalid refresh token.";
                public const string ExpiredToken = "Refresh token is expired.";
                public const string UserNotFound = "User not found.";
            }

            public static class Jwt
            {
                public const string InvalidConfiguration = "Invalid JWT configuration. Check Key, Issuer, and Audience settings.";
            }
        }

        public static class Account
        {
            public static class ChangePassword
            {
                public const string EmailNotFound = "Email does not exist.";
                public const string EmailNotVerified = "Email is not verified.";
                public const string AccountDisabled = "Account is disabled.";
            }
            public static class ForgotPassword
            {
                public const string EmailNotFound = "Email does not exist.";
                public const string EmailNotVerified = "Email is not verified.";
                public const string AccountDisabled = "Account is disabled.";
            }
            public static class ResetPassword
            {
                public const string EmailNotFound = "Email does not exist.";
                public const string EmailNotVerified = "Email is not verified.";
                public const string AccountDisabled = "Account is disabled.";
            }
        }

        public static class Email
        {
            public const string ConfirmSubject = "Confirm Your Email Address";
            public const string ForgotPasswordSubject = "Reset Your TellMe Password";
            public const string TemplateNotFound = "Email template not found.";
            public const string LogoImageNotFound = "Logo image not found.";
        }

        public static class Cache
        {
            public const string AccountTokenNotFound = "Account token not found in cache.";
        }
    }
}
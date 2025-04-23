using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Constants
{
    public static class MessageConstant
    {
        public static class LoginMessage
        {
            public const string NotExistEmail = "Email does not exist in the system.";
            public const string DisabledAccount = "Account has been disabled.";
            public const string NotVerifyEmail = "Email is not verified.";
            public const string InvalidEmailOrPassword = "Email or Password is invalid.";
        }
        public static class RegisterMessage
        {
            public const string AlreadyExistEmail = "Email already exists in the system.";
            public const string AlreadyExistAccount = "Email already exists in the system and you should login by email, password.";
            public const string RegistNewAccountFailure = "Have some thing error add user in system.";
        }
        public static class EmailMessage
        {
            public const string FileNotFound = "Email template not found.";
            public const string ConfirmSubject = "Confirm Your Email Address.";
        }
        public static class JWTMessage
        {
            public const string InvalidOperationJWT = "JWT configuration values are missing. Check JwtAuth:Key, JwtAuth:Issuer, and JwtAuth:Audience.";
        }
    }
}

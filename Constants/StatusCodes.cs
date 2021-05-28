
namespace AuthApiSesh.Constants
{
    public static class StatusCodTitle{
        public const string ErrorEmail = "Email is already registered.";
        public const string ErrorUsername = "Username is already registered.";
        public const string ErrorConfirm = "Account not verified.";
        public const string ErrorSignIn = "The username or password you entered is incorrect.";
        public const string ErrorPassword = "Password is incorrect.";
        public const string ErrorFnLn = "First name or last name incorrect.";
        public const string ErrorFirstName = "First name incorrect.";
        public const string ErrorLastName = "Last name incorrect.";
        public const string ErrorVerifLifetime = "Сode lifetime expired";
        public const string ErrorVerifCode = "Incorrect verification code";
    }
}
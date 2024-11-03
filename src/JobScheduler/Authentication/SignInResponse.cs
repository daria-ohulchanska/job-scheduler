namespace JobScheduler.Core.Authentication
{
    public class SignInResponse
    {
        AuthTokens Tokens { get; set; }
    }

    public class AuthTokens
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}

namespace JobScheduler.Core.Authentication;

public class SignInResponse
{
    public AuthenticationTokens Tokens { get; set; }
    public string UserName { get; set; }
}

public class AuthenticationTokens
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
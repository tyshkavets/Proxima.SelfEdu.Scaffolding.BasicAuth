namespace Proxima.SelfEdu.Scaffolding.BasicAuth;

/// <summary>
/// Base class for use in IOptions use case.
/// </summary>
public class BasicAuthenticationOptions
{
    /// <summary>
    /// Realm for HTTP Basic Authentication request.
    /// </summary>
    public string Realm { get; set; }
    
    /// <summary>
    /// Username that should be used for authentication. If not set, any username can be used for authentication to
    /// succeed and only password is verified.
    /// </summary>
    public string Username { get; set; }
    
    /// <summary>
    /// Password that satisfies authentication attempt.
    /// </summary>
    public string Password { get; set; }
}
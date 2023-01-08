using Microsoft.AspNetCore.Authentication;

namespace Proxima.SelfEdu.Scaffolding.BasicAuth;

/// <summary>
/// Configuration for Authentication Scheme that this package adds.
/// </summary>
public class BasicAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
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

    /// <summary>
    /// Callback for building Authentication Ticket.
    /// </summary>
    /// <returns></returns>
    public Func<string, AuthenticationTicket> AuthenticationTicketFactory { get; set; }

    public override void Validate()
    {
        base.Validate();

        if (string.IsNullOrEmpty(Realm))
        {
            throw new InvalidOperationException("Basic authentication handler configuration error - missing realm.");
        }

        if (string.IsNullOrEmpty(Password))
        {
            throw new InvalidOperationException("Basic authentication handler configuration error - missing password.");
        }
    }
}
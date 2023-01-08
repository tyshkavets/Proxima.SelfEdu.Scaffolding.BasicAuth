using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Defaults = Proxima.SelfEdu.Scaffolding.BasicAuth.BasicAuthenticationDefaults;

namespace Proxima.SelfEdu.Scaffolding.BasicAuth;

public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationSchemeOptions>
{
    private readonly ILogger<BasicAuthenticationHandler> _logger;

    public BasicAuthenticationHandler(
        IOptionsMonitor<BasicAuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
        _logger = logger.CreateLogger<BasicAuthenticationHandler>();
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorizationHeader = Request.Headers[Defaults.AuthHeader].ToString();
        if (string.IsNullOrEmpty(authorizationHeader))
        {
            return Fail("missing header", "Missing Authorization Header");
        }

        // Header should look like "Basic <Base64_value_here>"
        if (!authorizationHeader.StartsWith(Defaults.BasicPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return Fail("invalid header. Incorrect prefix.", "Invalid Authorization Header");
        }

        var encodedHeader = authorizationHeader[Defaults.BasicPrefix.Length..].Trim();
        string[] decodedHeader; 
        try
        {
            // Decoded Base64 part looks like username:password
            decodedHeader = Encoding.UTF8.GetString(Convert.FromBase64String(encodedHeader)).Split(':', 2);
        }
        catch (FormatException)
        {
            return Fail("invalid header. Could not parse Base64 header.", "Invalid Authorization Header");
        }

        var username = decodedHeader[0];
        var password = decodedHeader[1];

        if (!IsValidCredentials(username, password))
        {
            return Fail("invalid credentials", "Invalid Username or Password");
        }

        var ticket = Options.AuthenticationTicketFactory == null
            ? BuildTicket(username)
            : Options.AuthenticationTicketFactory(username); 

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private bool IsValidCredentials(string username, string password)
    {
        if (string.IsNullOrEmpty(Options.Username))
        {
            return Options.Password == password;
        }

        return Options.Username == username && Options.Password == password;
    }

    private AuthenticationTicket BuildTicket(string username)
    {
        var claims = new[] { new Claim(ClaimTypes.Name, username) };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);

        return new AuthenticationTicket(principal, Scheme.Name);
    }

    private Task<AuthenticateResult> Fail(string logMessage, string publicMessage)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.Headers.Add(Defaults.WwwAuthenticateHeader, $"{Defaults.BasicPrefix}realm=\"{Options.Realm}\"");
        _logger.LogInformation("Basic Authentication Handler: {LogMessage}", logMessage);
        return Task.FromResult(AuthenticateResult.Fail(publicMessage));
    }
}

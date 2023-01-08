using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Proxima.SelfEdu.Scaffolding.BasicAuth;

public static class ServiceCollectionExtensions
{
    public static AuthenticationBuilder AddBasicAuth(this IServiceCollection services,
        Action<BasicAuthenticationSchemeOptions> configureOptions)
    {
        return services.AddAuthentication(BasicAuthenticationDefaults.BasicAuthScheme)
            .AddScheme<BasicAuthenticationSchemeOptions, BasicAuthenticationHandler>(
                BasicAuthenticationDefaults.BasicAuthScheme, configureOptions);
    }

    public static AuthenticationBuilder AddBasicAuthFromOptions<TOptions>(
        this IServiceCollection services,
        string configSectionPath)
        where TOptions : BasicAuthenticationOptions
    {
        return services.AddBasicAuthFromOptions<TOptions>(configSectionPath, null);
    }
    
    public static AuthenticationBuilder AddBasicAuthFromOptions<TOptions>(
        this IServiceCollection services,
        string configSectionPath,
        Func<string, AuthenticationTicket> authTicketFactory)
        where TOptions : BasicAuthenticationOptions
    {
        services.AddOptions<TOptions>().BindConfiguration(configSectionPath);
        var authOptions = services.BuildServiceProvider().GetRequiredService<IOptions<TOptions>>().Value;

        return services.AddBasicAuth(options =>
        {
            options.Realm = authOptions.Realm;
            options.Username = authOptions.Username;
            options.Password = authOptions.Password;

            if (authTicketFactory != null)
            {
                options.AuthenticationTicketFactory = authTicketFactory;
            }
        });
    }
}

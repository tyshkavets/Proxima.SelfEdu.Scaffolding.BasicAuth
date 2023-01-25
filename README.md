# Proxima.SelfEdu.Scaffolding.BasicAuth
Handy project for quickly adding HTTP Basic Authentication to Asp.Net Core projects. Used for self-education purposes and experiments. Available on Nuget.

## Usage

Simplest way to enable basic authentication is to have configuration section in your `appsettings.json`

```json
"BasicAuthentication": {
    "Realm": "localhost",
    "Username": "admin",
    "Password": "your secret password here"
  }
```

Of course, you could use alternative configuration sources, such as environment variables,
which is especially convenient for docker compose scenarios. 

After that, you can add necessary services:

```csharp
builder.Services.AddBasicAuthFromOptions<BasicAuthenticationOptions>("BasicAuthentication");
```

Note that you remain in control of config section path (provided as a string parameter), as well as 
actual *Options object structure, as long as you can inherit from BasicAuthenticationOptions.

For more flexibility, you can use `.AddBasicAuth()` extension:

```csharp
builder.Services.AddBasicAuth(options =>
{
    options.Realm = "localhost";
    options.Username = "admin";
    options.Password = "your secret password here";
});
```

If you want to modify created `AuthenticationTicket`, for example to add custom claims into `ClaimsPrincipal`,
override `AuthenticationTicketFactory` delegate.

By default, auth scheme is created named `BasicAuthScheme`. If this by chance conflicts with your auth setup,
you can provide alternative scheme name as a parameter in existing overrides for `.AddBasicAuth` and
`.AddBasicAuthFromOptions` extensions.
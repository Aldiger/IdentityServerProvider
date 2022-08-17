using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Quartz;
using SignChainIdp.Api.Models;
using SignChainIdp.Api.Services;
using OpenIddict.UI.Api;
using OpenIddict.UI.Identity.Api;
using OpenIddict.UI.Identity.Infrastructure;
using OpenIddict.UI.Infrastructure;
using static OpenIddict.Abstractions.OpenIddictConstants;
using SignChainIdp.Api.Helpers.AppsettingConfigurationDto;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace SignChainIdp.Api;

public static class ConfigureServices
{
    public static IServiceCollection AddServer(
      this IServiceCollection services,
      IConfiguration configuration,
      string environmentName
    )
    {
        services.Configure<IdpConfiguration>(configuration.GetSection("IDP"));
        services.Configure<IdentityConfiguration>(configuration.GetSection("IDENTITY"));
        services.Configure<LocalizationConfiguration>(configuration.GetSection("Localization"));

        var idpConfiguration = configuration.GetSection("IDP").Get<IdpConfiguration>();
        var localizationConfiguration = configuration.GetSection("Localization").Get<LocalizationConfiguration>();
        var resourceConfiguration = GetResourceConfiguration(idpConfiguration);


        services.AddDbContext<ApplicationDbContext>(options =>
        {
            // Configure the context to use Microsoft SQL Server.
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        // Register the Identity services.
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Configure Identity to use the same JWT claims as OpenIddict instead
        // of the legacy WS-Federation claims it uses by default (ClaimTypes),
        // which saves you from doing the mapping in your authorization controller.
        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 5;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;

            options.ClaimsIdentity.UserNameClaimType = Claims.Name;
            options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
            options.ClaimsIdentity.RoleClaimType = Claims.Role;
        });

        //if (!Helpers.Constants.IsTestingEnvironment(environmentName))
        //{
        // OpenIddict offers native integration with Quartz.NET to perform scheduled tasks
        // (like pruning orphaned authorizations/tokens from the database) at regular intervals.
        services.AddQuartz(options =>
        {
            options.UseMicrosoftDependencyInjectionJobFactory();
            options.UseSimpleTypeLoader();
            options.UseInMemoryStore();
        });

        // Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
        //}

        services.AddOpenIddict()
          // Register the OpenIddict core components.
          .AddCore(options =>
          {
              options.UseEntityFrameworkCore();
              if (!Helpers.Constants.IsTestingEnvironment(environmentName))
              {
                  options.UseQuartz();
              }
          })
          // Register the OpenIddict server components.
          .AddServer(options =>
          {
              options.SetIssuer(new Uri(idpConfiguration.Issuer));

              // Enable the authorization, device, logout, token, userinfo and verification endpoints.
              options.SetAuthorizationEndpointUris("/connect/authorize")
                 .SetLogoutEndpointUris("/connect/logout")
                 .SetTokenEndpointUris("/connect/token")
                 .SetIntrospectionEndpointUris("/connect/introspect")
                 .SetUserinfoEndpointUris("/connect/userinfo");

              // Note: this sample uses the code, device, password and refresh token flows, but you
              // can enable the other flows if you need to support implicit or client credentials.
              options.AllowAuthorizationCodeFlow()
                  .AllowClientCredentialsFlow()
                 .AllowRefreshTokenFlow();

              // Mark the "email", "profile", "roles" and "demo_api" scopes as supported scopes.
              options.RegisterScopes(resourceConfiguration.Item1.ToArray());

              if (Helpers.Constants.IsTestingEnvironment(environmentName))
              {
                  // Register the signing and encryption credentials.
                  options.AddDevelopmentEncryptionCertificate()
                     .AddDevelopmentSigningCertificate();
              }
              else
              {
                  options.AddEphemeralEncryptionKey()
                     .AddEphemeralSigningKey();
              }

              // Force client applications to use Proof Key for Code Exchange (PKCE).
              options.RequireProofKeyForCodeExchange();

              // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
              options.UseAspNetCore()
                 .EnableStatusCodePagesIntegration()
                 .EnableAuthorizationEndpointPassthrough()
                 .EnableLogoutEndpointPassthrough()
                 .EnableTokenEndpointPassthrough()
                 .EnableUserinfoEndpointPassthrough();
              // .DisableTransportSecurityRequirement(); // During development, you can disable the HTTPS requirement.

              //if (configuration.GetValue("DisableAccessTokenEncryption", false))
              //{
              options.DisableAccessTokenEncryption();
              //}
          })
            .AddValidation(options =>
            {
                // Note: the validation handler uses OpenID Connect discovery
                // to retrieve the address of the introspection endpoint.
                options.SetIssuer(idpConfiguration.Issuer);
                options.AddAudiences(idpConfiguration.Audience);

                // Configure the validation handler to use introspection and register the client
                // credentials used when communicating with the remote introspection endpoint.
                options.UseIntrospection()
                        .SetClientId(idpConfiguration.IntrospectionClientId)
                        .SetClientSecret(idpConfiguration.IntrospectionClientSecret);

                // Register the System.Net.Http integration.
                options.UseSystemNetHttp();


                // Register the ASP.NET Core host.
                options.UseAspNetCore();
            })
          // Register the EF based UI Store for OpenIddict related entities.
          .AddUIStore(options =>
          {
              options.OpenIddictUIContext = builder =>
              {
                  builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                sql =>
                {
                    sql.MigrationsAssembly(typeof(Program)
                                      .GetTypeInfo()
                                      .Assembly
                                      .GetName()
                                      .Name);
                });
              };
          })
          // Register the APIs for the EF based UI Store based on OpenIddict.
          .AddUIApis(options =>
          {
              // Tell the system about the allowed Permissions it is built/configured for.
              options.Permissions = resourceConfiguration.Item2;
          })
          // Register the EF based UI Store for the ASP.NET Identity related entities.
          .AddUIIdentityStore<ApplicationUser>(options =>
          {
              options.OpenIddictUIIdentityContext = builder =>
              {
                  builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                sql =>
                {
                    sql.MigrationsAssembly(typeof(Program)
                                      .GetTypeInfo()
                                      .Assembly
                                      .GetName()
                                      .Name);
                });
              };
          })
          // Register the APIs for the EF based UI Store based on ASP.NET Identity.
          .AddUIIdentityApis<ApplicationUser>();

        //if (!Helpers.Constants.IsTestingEnvironment(environmentName))
        //{
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "API Server Documentation",
                Description = "API Server Documentation"
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Example: \"Bearer {token}\"",
                Type = SecuritySchemeType.ApiKey
            });
            c.DocInclusionPredicate((name, api) =>
            {
                return true;
            });
            c.TagActionsBy(api =>
            {
                if (api.GroupName != null)
                {
                    return new[] { api.GroupName };
                }

                if (api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                {
                    return new[] { controllerActionDescriptor.ControllerName };
                }

                throw new InvalidOperationException("Unable to determine tag for endpoint.");
            });
            c.ResolveConflictingActions(apiDescriptions =>
            {
                return apiDescriptions.First();
            });
        });
        //}

        services.AddScoped<IMigrationService, MigrationService>();

        services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

        services.AddPortableObjectLocalization(options => options.ResourcesPath = localizationConfiguration.FilesPath);

        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = localizationConfiguration.SupportedCultureInfo();

            options.DefaultRequestCulture = new RequestCulture(supportedCultures.First());
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });

        return services;
    }

    private static Tuple<List<string>, List<string>> GetResourceConfiguration(IdpConfiguration idpConfiguration)
    {

        List<string> registerScopeList = new List<string>
              {
                    Scopes.OpenId,
                    Scopes.Email,
                    Scopes.Profile,
                    Scopes.Roles
              };
        foreach (var item in idpConfiguration.Scopes)
        {
            registerScopeList.Add(item.Name);
        }

        List<string> permissionsList = new List<string>
        {
          Permissions.Endpoints.Authorization,
          Permissions.Endpoints.Logout,
          Permissions.Endpoints.Token,
          Permissions.Endpoints.Introspection,
          Permissions.GrantTypes.AuthorizationCode,
          Permissions.GrantTypes.DeviceCode,
          Permissions.GrantTypes.RefreshToken,
          Permissions.ResponseTypes.Code,
          Permissions.Scopes.Email,
          Permissions.Scopes.Profile,
          Permissions.Scopes.Roles,
        };

        foreach (var item in idpConfiguration.Scopes)
        {
            registerScopeList.Add(Permissions.Prefixes.Scope + item.Name);
        }

        return Tuple.Create(registerScopeList, permissionsList);
    }
}

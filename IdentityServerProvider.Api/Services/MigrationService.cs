using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using IdentityServerProvider.Api.Models;
using OpenIddict.UI.Identity.Infrastructure;
using OpenIddict.UI.Infrastructure;
using OpenIddict.UI.Suite.Core;
using IdentityServerProvider.Api.Helpers.AppsettingConfigurationDto;
using Microsoft.Extensions.Options;

namespace IdentityServerProvider.Api.Services;

public interface IMigrationService
{
    Task EnsureMigrationAsync();
}

public class MigrationService : IMigrationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IdpConfiguration _idpConfigurations;
    private readonly IdentityConfiguration _identityConfigurations;

    public MigrationService(
        IServiceProvider serviceProvider,
        IOptions<IdpConfiguration> idpConfigurations,
        IOptions<IdentityConfiguration> identityConfigurations
        )
    {
        _serviceProvider = serviceProvider;
        _identityConfigurations = identityConfigurations.Value;
        _idpConfigurations = idpConfigurations.Value;
    }

    public async Task EnsureMigrationAsync()
    {
        using var scope = _serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        var uiContext = scope.ServiceProvider.GetRequiredService<OpenIddictUIContext>();
        await uiContext.Database.MigrateAsync();

        var uiIdentityContext = scope.ServiceProvider.GetRequiredService<OpenIddictUIIdentityContext>();
        await uiIdentityContext.Database.MigrateAsync();

        await RegisterApplicationsAsync(scope.ServiceProvider, _idpConfigurations);

        await RegisterScopesAsync(scope.ServiceProvider, _idpConfigurations);

        await RegisterRoleAsync(scope.ServiceProvider, _identityConfigurations);

        await EnsureAdministratorRole(scope.ServiceProvider);

        await EnsureAdministratorUser(scope.ServiceProvider, _identityConfigurations);
    }


    private static async Task RegisterApplicationsAsync(IServiceProvider provider, IdpConfiguration idpConfigurations)
    {
        var manager = provider.GetRequiredService<IOpenIddictApplicationManager>();

        foreach (var client in idpConfigurations.Clients)
        {
            if (await manager.FindByClientIdAsync(client.ClientId) is null)
            {
                await manager.CreateAsync(client);
            }
        }
    }

    private static async Task RegisterScopesAsync(IServiceProvider provider, IdpConfiguration idpConfigurations)
    {
        var manager = provider.GetRequiredService<IOpenIddictScopeManager>();

        foreach (var scope in idpConfigurations.Scopes)
        {
            if (await manager.FindByNameAsync(scope.Name) is null)
            {
                await manager.CreateAsync(scope);
                
            }
        }
    }

    private static async Task EnsureAdministratorRole(IServiceProvider provider)
    {
        var manager = provider.GetRequiredService<RoleManager<IdentityRole>>();

        var role = Roles.SuperAdmin;
        var roleExists = await manager.RoleExistsAsync(role);
        if (!roleExists)
        {
            var newRole = new IdentityRole(role);
            await manager.CreateAsync(newRole);
        }
    }

    private static async Task RegisterRoleAsync(IServiceProvider provider, IdentityConfiguration _identityConfiguration)
    {
        var manager = provider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var item in _identityConfiguration.Roles)
        {
            if (!await manager.RoleExistsAsync(item.Name))
            {
                await manager.CreateAsync(new IdentityRole(item.Name));
            }

        }
    }


    private static async Task EnsureAdministratorUser(IServiceProvider provider, IdentityConfiguration identityConfigurations)
    {
        var manager = provider.GetRequiredService<UserManager<ApplicationUser>>();
        foreach (var admin in identityConfigurations.Users.Admins)
        {
            var user = await manager.FindByNameAsync(admin.Email);
            if (user != null)
            {
                return;
            }

            var applicationUser = new ApplicationUser
            {
                UserName = admin.UserName,
                Email = admin.Email,
            };

            var userResult = await manager.CreateAsync(applicationUser, admin.Password);
            if (!userResult.Succeeded)
            {
                return;
            }

            await manager.SetLockoutEnabledAsync(applicationUser, false);
            await manager.AddToRoleAsync(applicationUser, Roles.SuperAdmin);
        }
    }
}

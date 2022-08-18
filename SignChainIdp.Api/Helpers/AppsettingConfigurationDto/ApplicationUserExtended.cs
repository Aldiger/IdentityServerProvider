using IdentityServerProvider.Api.Models;

namespace IdentityServerProvider.Api.Helpers.AppsettingConfigurationDto
{
    public class ApplicationUserExtended : ApplicationUser
    {
        public string? Password { get; set; }
    }
}

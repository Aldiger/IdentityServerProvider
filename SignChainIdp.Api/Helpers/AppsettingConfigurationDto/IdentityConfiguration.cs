using Microsoft.AspNetCore.Identity;

namespace IdentityServerProvider.Api.Helpers.AppsettingConfigurationDto
{
    public class IdentityConfiguration
    {
        public UserConfiguration? Users { get; set; }
        public List<IdentityRole>? Roles { get; set; }
    }

}

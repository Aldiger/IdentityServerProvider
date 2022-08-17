using SignChainIdp.Api.Models;

namespace SignChainIdp.Api.Helpers.AppsettingConfigurationDto
{
    public class ApplicationUserExtended : ApplicationUser
    {
        public string? Password { get; set; }
    }
}

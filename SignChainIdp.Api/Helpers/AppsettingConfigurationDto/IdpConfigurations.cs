using OpenIddict.Abstractions;

namespace IdentityServerProvider.Api.Helpers.AppsettingConfigurationDto
{
    public class IdpConfiguration
    {
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? IntrospectionClientId { get; set; }
        public string? IntrospectionClientSecret { get; set; }

        public string? InternalClientId { get; set; }
        public string? InternalClientSecret { get; set; }
        public IList<OpenIddictScopeDescriptor>? Scopes { get; set; }
        public IList<OpenIddictApplicationDescriptor>? Clients { get; set; }
    }
}

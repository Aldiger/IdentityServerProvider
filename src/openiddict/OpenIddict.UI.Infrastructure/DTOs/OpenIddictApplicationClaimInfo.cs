using System;

namespace OpenIddict.UI.Infrastructure;

public class OpenIddictApplicationClaimInfo
{
    public Guid Id { get; set; }

    public string ApplicationId { get; set; }

    public string ClaimType { get; set; }

    public string ClaimValue { get; set; }

}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenIddict.UI.Infrastructure;

public interface IOpenIddictApplicationClaimService
{
    Task<IEnumerable<OpenIddictApplicationClaimInfo>> GetOpenIddictApplicationClaimAsync(string applicationId);
    Task RemoveApplicationClaimsAsync(string applicationId);
    Task AddApplicationClaimsAsync(List<OpenIddictApplicationClaim> claims, string applicationId);
}

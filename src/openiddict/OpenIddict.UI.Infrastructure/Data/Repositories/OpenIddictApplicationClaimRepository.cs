using System;
using OpenIddict.UI.Infrastructure;

namespace OpenIddict.UI.Infrastructure;

public class OpenIddictApplicationClaimRepository<TContext>
  : EfRepository<OpenIddictApplicationClaim, Guid>, IOpenIddictApplicationClaimRepository
  where TContext : OpenIddictUIContext
{
    public OpenIddictApplicationClaimRepository(TContext dbContext) : base(dbContext)
    {
    }
}

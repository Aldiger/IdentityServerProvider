using System;
using OpenIddict.UI.Identity.Core;

namespace OpenIddict.UI.Identity.Infrastructure;

public class ClaimTypeRepository<TContext>
  : EfRepository<ClaimType, Guid>, IClaimTypeRepository
  where TContext : OpenIddictUIIdentityContext
{
  public ClaimTypeRepository(TContext dbContext) : base(dbContext)
  {
  }
}

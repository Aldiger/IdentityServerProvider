using OpenIddict.EntityFrameworkCore.Models;

namespace OpenIddict.UI.Infrastructure;

public class ScopeRepository<TContext>
  : EfRepository<OpenIddictEntityFrameworkCoreScope, string>, IScopeRepository
  where TContext : OpenIddictUIContext
{
  public ScopeRepository(TContext dbContext) : base(dbContext)
  {
  }
}

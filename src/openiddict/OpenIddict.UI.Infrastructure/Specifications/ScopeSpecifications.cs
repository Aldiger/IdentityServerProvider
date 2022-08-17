using OpenIddict.EntityFrameworkCore.Models;
using OpenIddict.UI.Suite.Core;

namespace OpenIddict.UI.Infrastructure;

public sealed class AllScopes : BaseSpecification<OpenIddictEntityFrameworkCoreScope>
{
  public AllScopes()
  {
    ApplyOrderBy(x => x.Name);
    ApplyNoTracking();
  }
}

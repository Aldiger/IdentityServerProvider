using OpenIddict.EntityFrameworkCore.Models;
using OpenIddict.UI.Suite.Core;

namespace OpenIddict.UI.Infrastructure;

public sealed class AllApplications : BaseSpecification<OpenIddictEntityFrameworkCoreApplication>
{
  public AllApplications()
  {
    ApplyOrderBy(x => x.DisplayName);
    ApplyNoTracking();
  }
}

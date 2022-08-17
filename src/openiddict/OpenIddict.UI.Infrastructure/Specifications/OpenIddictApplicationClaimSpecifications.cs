using OpenIddict.UI.Infrastructure;
using OpenIddict.UI.Suite.Core;
using System;

namespace OpenIddict.UI.Infrastructure;

public sealed class AllOpenIddictApplicationClaimByApplications : BaseSpecification<OpenIddictApplicationClaim>
{
  public AllOpenIddictApplicationClaimByApplications(string applicationId)
  {
        AddCriterion(x => x.ApplicationId == applicationId);
        ApplyOrderBy(x => x.ClaimType);
        ApplyNoTracking();
  }
}


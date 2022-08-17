using System;
using OpenIddict.UI.Infrastructure;
using OpenIddict.UI.Suite.Core;

namespace OpenIddict.UI.Infrastructure;

public interface IOpenIddictApplicationClaimRepository : IAsyncRepository<OpenIddictApplicationClaim, Guid>
{ }

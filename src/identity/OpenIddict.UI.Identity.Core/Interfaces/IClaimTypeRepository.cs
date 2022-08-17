using System;
using OpenIddict.UI.Suite.Core;

namespace OpenIddict.UI.Identity.Core;

public interface IClaimTypeRepository : IAsyncRepository<ClaimType, Guid>
{ }

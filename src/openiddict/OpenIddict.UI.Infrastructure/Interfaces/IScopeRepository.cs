using OpenIddict.EntityFrameworkCore.Models;
using OpenIddict.UI.Suite.Core;

namespace OpenIddict.UI.Infrastructure;

public interface IScopeRepository
  : IAsyncRepository<OpenIddictEntityFrameworkCoreScope, string>
{ }

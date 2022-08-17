using OpenIddict.EntityFrameworkCore.Models;
using OpenIddict.UI.Suite.Core;

namespace OpenIddict.UI.Infrastructure;

public interface IApplicationRepository
  : IAsyncRepository<OpenIddictEntityFrameworkCoreApplication, string>
{ }

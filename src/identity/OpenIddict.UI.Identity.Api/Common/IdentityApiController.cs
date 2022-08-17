using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using OpenIddict.UI.Suite.Api;

namespace OpenIddict.UI.Identity.Api;

[ApiExplorerSettings(GroupName = ApiGroups.OPENIDDICTUIIDENTITYGROUP)]
[Authorize(
  //Policies.OPENIDDICTUIIDENTITYAPIPOLICY,
  AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme
)]
public class IdentityApiController : ApiControllerBase
{
}

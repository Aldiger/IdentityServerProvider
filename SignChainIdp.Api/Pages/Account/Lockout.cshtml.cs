using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServerProvider.Api.Pages.Account;

[AllowAnonymous]
public class LockoutModel : PageModel
{
  public void OnGet()
  {

  }
}

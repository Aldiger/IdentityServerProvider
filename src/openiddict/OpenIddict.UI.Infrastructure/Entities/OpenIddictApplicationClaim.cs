using OpenIddict.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIddict.UI.Infrastructure;

public class OpenIddictApplicationClaim
{
    public Guid Id { get; set; }

    public string ApplicationId { get; set; }

    public string ClaimType { get; set; }

    public string ClaimValue { get; set; }

    public static OpenIddictApplicationClaim Create(
      string applicationid,
      string claimTypeName,
      string claimTypeValue
    )
    {
        return new OpenIddictApplicationClaim
        {
            Id = Guid.NewGuid(),
            ApplicationId = applicationid,
            ClaimType = claimTypeName,
            ClaimValue = claimTypeValue
        };
    }

    public OpenIddictEntityFrameworkCoreApplication Application { get; set; }
}


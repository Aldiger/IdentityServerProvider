using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenIddict.UI.Suite.Core;

namespace OpenIddict.UI.Infrastructure;

public class OpenIddictApplicationClaimService : IOpenIddictApplicationClaimService
{
    private readonly IOpenIddictApplicationClaimRepository _repository;

    public OpenIddictApplicationClaimService(IOpenIddictApplicationClaimRepository repository)
    {
        _repository = repository
          ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IEnumerable<OpenIddictApplicationClaimInfo>> GetOpenIddictApplicationClaimAsync(string applicationId)
    {
        var result = await _repository.ListAsync(new AllOpenIddictApplicationClaimByApplications(applicationId));
        return result.Select(x => ToInfo(x)).ToList();
    }

    public async Task RemoveApplicationClaimsAsync(string applicationId)
    {
        var claims = await _repository.ListAsync(new AllOpenIddictApplicationClaimByApplications(applicationId));

        await _repository.DeleteRangeAsync(claims.ToList());

    }

    public async Task AddApplicationClaimsAsync(List<OpenIddictApplicationClaim> claims, string applicationId)
    {

        claims.ForEach(x => x.ApplicationId = applicationId);
        await _repository.AddRangeAsync(claims);
    }
    private static OpenIddictApplicationClaimInfo ToInfo(OpenIddictApplicationClaim entity) => SimpleMapper.From<OpenIddictApplicationClaim, OpenIddictApplicationClaimInfo>(entity);
}

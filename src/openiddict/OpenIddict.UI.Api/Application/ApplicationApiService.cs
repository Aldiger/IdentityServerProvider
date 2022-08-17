using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenIddict.UI.Infrastructure;
using OpenIddict.UI.Suite.Core;

namespace OpenIddict.UI.Api;

public interface IApplicationApiService
{
    Task<IEnumerable<ApplicationViewModel>> GetApplicationsAsync();

    Task<ApplicationViewModel> GetAsync(string clientId);

    Task<string> CreateAsync(ApplicationViewModel model);

    Task UpdateAsync(ApplicationViewModel model);

    Task DeleteAsync(string clientId);

    Task<ApplicationOptionsViewModel> GetOptionsAsync();
}

public class ApplicationApiService : IApplicationApiService
{
    private readonly IApplicationService _service;
    private readonly IOptions<OpenIddictUIApiOptions> _options;

    public ApplicationApiService(
      IApplicationService service,
      IOptions<OpenIddictUIApiOptions> options
    )
    {
        _service = service
          ?? throw new ArgumentNullException(nameof(service));
        _options = options
          ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<IEnumerable<ApplicationViewModel>> GetApplicationsAsync()
    {
        var items = await _service.GetApplicationsAsync();

        return items.Select(c =>
        {
            return ToModel(c);
        });
    }

    public async Task<ApplicationViewModel> GetAsync(string clientId)
    {
        if (clientId == null)
        {
            throw new ArgumentNullException(nameof(clientId));
        }

        var application = await _service.GetAsync(clientId);

        if (application == null) return null;

        var result = ToModel(application);
        result.Claims = application.Claims.Select(x =>
        new ApplicationClaimViewModel
        {
            Type = x.ClaimType,
            Value = x.ClaimValue
        }).ToList();
        return result;
    }

    public async Task<string> CreateAsync(ApplicationViewModel model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        var param = ToParam(model);

        return await _service.CreateAsync(param);
    }

    public async Task UpdateAsync(ApplicationViewModel model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        if (string.IsNullOrWhiteSpace(model.Id))
        {
            throw new InvalidOperationException(nameof(model.Id));
        }

        var param = ToParam(model);
        param.Claims = model.Claims.Select(x =>
        new OpenIddictApplicationClaimInfo
        {
            ClaimType = x.Type,
            ClaimValue = x.Value
        }).ToList();

        await _service.UpdateAsync(param);
    }

    public async Task DeleteAsync(string clientId)
    {
        if (clientId == null)
        {
            throw new ArgumentNullException(nameof(clientId));
        }

        await _service.DeleteAsync(clientId);
    }

    public async Task<ApplicationOptionsViewModel> GetOptionsAsync()
    {
        var options = _options.Value;

        var model = new ApplicationOptionsViewModel
        {
            Permissions = options.Permissions
        };

        return await Task.FromResult(model);
    }

    private static ApplicationParam ToParam(ApplicationViewModel model) => SimpleMapper.From<ApplicationViewModel, ApplicationParam>(model);

    private static ApplicationViewModel ToModel(ApplicationInfo info) => SimpleMapper.From<ApplicationInfo, ApplicationViewModel>(info);
}

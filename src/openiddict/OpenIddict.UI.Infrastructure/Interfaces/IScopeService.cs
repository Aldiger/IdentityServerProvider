using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenIddict.UI.Infrastructure;

public interface IScopeService
{
  Task<IEnumerable<ScopeInfo>> GetScopesAsync();

  Task<ScopeInfo> GetAsync(string id);

  Task<string> CreateAsync(ScopeParam model);

  Task UpdateAsync(ScopeParam model);

  Task DeleteAsync(string id);
}

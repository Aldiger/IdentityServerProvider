using OpenIddict.Abstractions;
using System.Globalization;

namespace SignChainIdp.Api.Helpers.AppsettingConfigurationDto
{
    public class LocalizationConfiguration
    {
        public string? FilesPath { get; set; }
        public string? SupportedCultures { get; set; }

        public List<CultureInfo> SupportedCultureInfo()
        {
           return SupportedCultures.Split(',').Select(c => new CultureInfo(c)).ToList();
        }
    }
}

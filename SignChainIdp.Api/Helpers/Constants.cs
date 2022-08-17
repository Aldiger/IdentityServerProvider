using System;
namespace SignChainIdp.Api.Helpers;

public static class Constants
{
    public static bool IsDevelopmentEnvironment(string environmentName) =>  !environmentName.Contains("Prod");

    public static bool IsTestingEnvironment(string environmentName) => !environmentName.Contains("Azure");
}

using System;

using Microsoft.Extensions.Configuration;

namespace BitChopp;

public class ConfigService(IConfiguration configuration)
{
    private readonly IConfiguration _configuration = configuration;

    public string GetApiKey()
    {
        return _configuration["ApiKey"] ?? throw new Exception("Missing ApiKey config");
    }

    public string GetSwitchId()
    {
        return _configuration["SwitchId"] ?? throw new Exception("Missing SwitchId config");
    }

    public Uri GetLnBitsHost()
    {
        var host = _configuration["LnBitsHost"] ?? throw new Exception("Missing LNbitsHost config");

        return new Uri(host);
    }
}

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

    public Uri GetWsHost()
    {
        var lnbitsHost = this.GetLnBitsHost();
        var deviceId = this.GetSwitchId();
        var wsHost = lnbitsHost.ToString().Replace("http", "ws");
        return new Uri($"{wsHost}api/v1/ws/{deviceId}");
    }

    public bool IsKiosk()
    {
        return _configuration["IsKiosk"] == "1" || _configuration["IsKiosk"]?.ToLowerInvariant() == "true";
    }
}

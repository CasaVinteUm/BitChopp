using Microsoft.Extensions.Configuration;

namespace BitChopp;

public class ConfigService
{
    private readonly IConfiguration _configuration;

    public ConfigService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string? GetApiKey()
    {
        return _configuration["ApiKey"];
    }
}

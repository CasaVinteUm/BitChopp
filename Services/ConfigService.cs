using Microsoft.Extensions.Configuration;

namespace BitChopp;

public class ConfigService(IConfiguration configuration)
{
    private readonly IConfiguration _configuration = configuration;

    public string? GetApiKey()
    {
        return _configuration["ApiKey"];
    }
}

using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using BitChopp.Models;

namespace BitChopp;

public class ApiService
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly ConfigService _configService;

    private readonly HttpClient _client = new();

    public ApiService(ConfigService configService)
    {
        _configService = configService;
        _client.BaseAddress = configService.GetLnBitsHost();
        _client.DefaultRequestHeaders.Add("X-API-Key", configService.GetApiKey());
    }

    public async Task<List<LnUrlPosDevice>?> FetchLnurlPos()
    {
        var json = await _client.GetStringAsync("/lnurldevice/api/v1/lnurlpos");
        var items = JsonSerializer.Deserialize<List<LnUrlPosDevice>>(json, _options);

        return items;
    }
}

using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BitChopp;

public class ApiService(ConfigService configService)
{
    private readonly ConfigService _configService = configService;
    public string? ApiResponse { get; private set; }

    public async Task<List<ApiResponseItem>?> FetchDataAsync()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("X-API-Key", _configService.GetApiKey());

        var json = await client.GetStringAsync("https://lnbits.casa21.space/lnurldevice/api/v1/lnurlpos");
        var items = JsonSerializer.Deserialize<List<ApiResponseItem>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return items;
    }
}
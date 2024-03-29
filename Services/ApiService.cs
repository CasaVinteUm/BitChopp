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

        //"[{\"id\":\"AgDLfdgTyr6E5H8i3VMHQo\",\"key\":\"RRXP9zePRmaNV6VNuNr3p9\",\"title\":\"TEST\",\"wallet\":\"31850178f739497da751cca8fee89fbf\",\"profit\":1.0,\"currency\":\"BRL\",\"device\":\"switch\",\"switches\":[{\"amount\":7.5,\"duration\":1000,\"pin\":0,\"comment\":true,\"variable\":false,\"lnurl\":\"LNURL1DP68GURN8GHJ7MRWVF5HGUEWVDSHXCFJXYH8XURPVDJJ7MRWW4EXCER9WE5KXEF0V9CXJTMKXGHKCMN4WFKZ7ST8G3XXVER823UHYDJ9X4YRS6FN2EX5S5T08ACXJM3AXQNXZMT0W4H8G0FH9C6JVER4WFSHG6T0DC7NZVPSXQN8VCTJD9SKYMR984RXZMRNV5NXXMMDD4JKUAPA23E82EGHAZJ5E\"},{\"amount\":5.0,\"duration\":1000,\"pin\":1,\"comment\":true,\"variable\":false,\"lnurl\":\"LNURL1DP68GURN8GHJ7MRWVF5HGUEWVDSHXCFJXYH8XURPVDJJ7MRWW4EXCER9WE5KXEF0V9CXJTMKXGHKCMN4WFKZ7ST8G3XXVER823UHYDJ9X4YRS6FN2EX5S5T08ACXJM3AXYNXZMT0W4H8G0F49CCZVER4WFSHG6T0DC7NZVPSXQN8VCTJD9SKYMR984RXZMRNV5NXXMMDD4JKUAPA23E82EGX9LU0M\"},{\"amount\":10.0,\"duration\":1000,\"pin\":3,\"comment\":true,\"variable\":false,\"lnurl\":\"LNURL1DP68GURN8GHJ7MRWVF5HGUEWVDSHXCFJXYH8XURPVDJJ7MRWW4EXCER9WE5KXEF0V9CXJTMKXGHKCMN4WFKZ7ST8G3XXVER823UHYDJ9X4YRS6FN2EX5S5T08ACXJM3AXVNXZMT0W4H8G0F3XQHRQFNYW4EXZARFDAHR6VFSXQCZVANPWF5KZCNVV575VCTVWDJJVCM0D4KK2MN58428YAT9WS23CM\"}],\"timestamp\":\"1711651296\"}]";
        var json = await client.GetStringAsync("https://lnbits.casa21.space/lnurldevice/api/v1/lnurlpos");
        var items = JsonSerializer.Deserialize<List<ApiResponseItem>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return items;
    }
}
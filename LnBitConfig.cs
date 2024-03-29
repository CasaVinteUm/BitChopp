using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BitChopp;

public class ApiResponseItem
{
    public string? Id { get; set; }
    public string? Key { get; set; }
    public string? Title { get; set; }
    public string? Wallet { get; set; }
    public double Profit { get; set; }
    public string? Currency { get; set; }
    public string? Device { get; set; }
    public List<Switch>? Switches { get; set; }
    public string? Timestamp { get; set; }
}

public class Switch
{
    public double Amount { get; set; }
    public int Duration { get; set; }
    public int Pin { get; set; }
    public string? Lnurl { get; set; }

    [JsonIgnore]
    public string? Description { get; set; }
}
using System.Collections.Generic;

namespace BitChopp.Models;

public class LnUrlPosDevice
{
    public string? Id { get; set; }
    public string? Key { get; set; }
    public string? Title { get; set; }
    public string? Wallet { get; set; }
    public double Profit { get; set; }
    public string? Currency { get; set; }
    public string? Device { get; set; }
    public List<LnUrlPosSwitch> Switches { get; set; } = [];
    public string? Timestamp { get; set; }
}

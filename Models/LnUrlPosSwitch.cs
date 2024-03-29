using System.Text.Json.Serialization;

namespace BitChopp.Models;

public class LnUrlPosSwitch
{
    public double Amount { get; set; }
    public int Duration { get; set; }
    public int Pin { get; set; }
    public string? Lnurl { get; set; }
    public string? Description { get; set; }
}

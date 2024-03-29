namespace BitChopp.Models;

public class LnUrlPosSwitch
{
    public double Amount { get; set; }
    public int Duration { get; set; }
    public int Pin { get; set; }
    public required string Lnurl { get; set; }
    public required string Description { get; set; }
}

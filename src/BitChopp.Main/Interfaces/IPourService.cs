namespace BitChopp.Main.Interfaces;

public interface IPourService
{
    int FlowCounter { get; }
    event EventHandler<int>? FlowCounterUpdated;
    event EventHandler<bool>? PourEnded;

    void PourExactly(int milliliters);
    public void OpenValve();
    public void CloseValve();
    public void CleanIO();
}
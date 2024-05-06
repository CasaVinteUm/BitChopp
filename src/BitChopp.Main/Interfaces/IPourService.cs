namespace BitChopp.Main.Interfaces;

public interface IPourService : IDisposable
{
    double FlowCounter { get; }
    event EventHandler<double>? FlowCounterUpdated;
    event EventHandler<bool>? PourEnded;

    void PourExactly(int milliliters);
    public void OpenValve();
    public void CloseValve();
}
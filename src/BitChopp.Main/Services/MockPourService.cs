namespace BitChopp.Main.Services;

using Interfaces;

public class MockPourService : IPourService
{
    public int FlowCounter { get; private set; }

    public event EventHandler<int>? FlowCounterUpdated;
    public event EventHandler<bool>? PourEnded;

    public async void PourExactly(int milliliters)
    {
        FlowCounter = 0;

        OpenValve();

        for (var i = 0; i < milliliters; i++)
        {
            FlowCounter++;
            FlowCounterUpdated?.Invoke(this, FlowCounter);
            await Task.Delay(50); // Simulate flow increment delay
        }

        CloseValve();

        PourEnded?.Invoke(this, true);
    }

    public void CleanIO()
    {
        Console.WriteLine("Mock: IO Cleaned.");
    }

    public void CloseValve()
    {
        Console.WriteLine("Mock: Valve closed.");
    }

    public void OpenValve()
    {
        Console.WriteLine("Mock: Valve opened.");
    }
}
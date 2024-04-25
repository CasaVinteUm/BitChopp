namespace BitChopp.Main.Services;

using System.Timers;
using Interfaces;

public class MockPourService : IPourService
{
    private readonly Timer _flowTimer;

    private bool _isValveOpen;

    public int FlowCounter { get; private set; }

    public event EventHandler<int>? FlowCounterUpdated;
    public event EventHandler<bool>? PourEnded;

    public MockPourService()
    {
        _flowTimer = new Timer(TimeSpan.FromMilliseconds(100)); // Set the interval to 50 milliseconds
        _flowTimer.Elapsed += OnFlowTimerElapsed;
    }

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
        _flowTimer.Stop();
        _isValveOpen = false;
        Console.WriteLine("Mock: Valve closed.");
    }

    public void OpenValve()
    {
        _flowTimer.Start();
        _isValveOpen = true;
        Console.WriteLine("Mock: Valve opened.");
    }

    private void OnFlowTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (_isValveOpen)
        {
            FlowCounter++;
            FlowCounterUpdated?.Invoke(this, FlowCounter);
        }
    }
}
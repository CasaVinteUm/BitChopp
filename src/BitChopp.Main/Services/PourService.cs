using System.Device.Gpio;

namespace BitChopp.Main.Services;

using Avalonia.Threading;
using Interfaces;

public class PourService : IPourService
{
    private readonly GpioController _gpioController;
    private readonly int _valvePin = 40;
    private readonly int _flowSensorPin = 11;
    private readonly ConfigService _configService;

    private bool _isValveOpen = false;

    public double FlowCounter { get; private set; }
    public event EventHandler<double>? FlowCounterUpdated;
    public event EventHandler<bool>? PourEnded;

    public PourService(ConfigService configService)
    {
        Console.WriteLine("PourService constructor");
        _configService = configService;
        _gpioController = new GpioController(PinNumberingScheme.Board);

        _valvePin = configService.ValvePin();
        _flowSensorPin = configService.FlowSensorPin();

        _gpioController.OpenPin(_valvePin, PinMode.Output);
        _gpioController.Write(_valvePin, PinValue.Low);

        _gpioController.OpenPin(_flowSensorPin, PinMode.Input);
        _gpioController.RegisterCallbackForPinValueChangedEvent(_flowSensorPin, PinEventTypes.Falling | PinEventTypes.Rising, HandleFlow);
    }

    public async void PourExactly(int milliliters)
    {
        Console.WriteLine("PourExactly {0} ml", milliliters);
        FlowCounter = 0;

        OpenValve();

        while (FlowCounter < milliliters)
        {
            if (milliliters - FlowCounter < 10)
            {
                _ = Dispatcher.UIThread.InvokeAsync(() =>
                {
                    PourEnded?.Invoke(this, true);
                    Console.WriteLine("Pour:PourEnded");
                });
            }
            await Task.Yield();
        }

        _ = Dispatcher.UIThread.InvokeAsync(() =>
        {
            PourEnded?.Invoke(this, true);
            Console.WriteLine("Pour:PourEnded");
        });

        CloseValve();

        Console.WriteLine("PourEnded");
    }

    public void OpenValve()
    {
        if (!_isValveOpen)
        {
            Console.WriteLine("OpenValve");
            _gpioController.Write(_valvePin, PinValue.High);
            _isValveOpen = true;
        }
    }

    public void CloseValve()
    {
        if (_isValveOpen)
        {
            Console.WriteLine("CloseValve");
            _gpioController.Write(_valvePin, PinValue.Low);
            _isValveOpen = false;
        }
    }

    private void CleanIO()
    {
        _gpioController.ClosePin(_valvePin);
        _gpioController.ClosePin(_flowSensorPin);
        _gpioController.Dispose();
    }

    private void HandleFlow(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
    {
        if (pinValueChangedEventArgs.PinNumber == 17)
        {
            FlowCounter += 1D / _configService.PulsesPerMl();
            Console.WriteLine("HandleFlow:FlowCounter: {0}", FlowCounter);
            Dispatcher.UIThread.InvokeAsync(() => FlowCounterUpdated?.Invoke(this, FlowCounter));
        }
    }

    #region Dispose Pattern
    private bool _disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            CleanIO();
        }

        _disposed = true;
    }

    ~PourService()
    {
        CleanIO();
    }
    #endregion
}
using System.Device.Gpio;

namespace BitChopp.Main.Services;

public class PourService
{
    private readonly GpioController _gpioController;
    private readonly int _valvePin = 40;
    private readonly int _flowSensorPin = 11;

    private bool _isValveOpen = false;

    public int FlowCounter { get; private set; }

    public PourService(ConfigService configService)
    {
        _gpioController = new GpioController(PinNumberingScheme.Board);

        _valvePin = configService.ValvePin();
        _flowSensorPin = configService.FlowSensorPin();

        _gpioController.OpenPin(_valvePin, PinMode.Output);
        _gpioController.Write(_valvePin, PinValue.Low);

        _gpioController.OpenPin(_flowSensorPin, PinMode.Input);
        _gpioController.RegisterCallbackForPinValueChangedEvent(_flowSensorPin, PinEventTypes.Rising | PinEventTypes.Falling, HandleFlow);
    }

    public void PourExactly(int milliliters)
    {
        FlowCounter = 0;

        OpenValve();

        while (FlowCounter < milliliters)
        {
            // Wait for the flow sensor to count the desired amount of milliliters
            // Maybe raise an event?
        }

        CloseValve();
    }

    public void OpenValve()
    {
        if (!_isValveOpen)
        {
            _gpioController.Write(_valvePin, PinValue.High);
            _isValveOpen = true;
        }
    }

    public void CloseValve()
    {
        if (_isValveOpen)
        {
            _gpioController.Write(_valvePin, PinValue.Low);
            _isValveOpen = false;
        }
    }

    public void CleanIO()
    {
        _gpioController.ClosePin(_valvePin);
        _gpioController.ClosePin(_flowSensorPin);
        _gpioController.Dispose();
    }

    private void HandleFlow(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
    {
        if (pinValueChangedEventArgs.PinNumber == _flowSensorPin)
        {
            FlowCounter++;
        }
    }
}
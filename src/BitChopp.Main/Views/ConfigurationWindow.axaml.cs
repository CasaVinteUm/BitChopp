using Avalonia.Interactivity;
using Avalonia.Threading;

namespace BitChopp.Main.Views;

using Interfaces;
using Services;

public partial class ConfigurationWindow : KioskBaseWindow
{
    private readonly IPourService _pourService;

    private double _initialFlowCount;

    private readonly System.Timers.Timer _updateTimer;
    private double _latestFlowCount;
    private bool _updatePending = false;

    public ConfigurationWindow(ConfigService configService) : base(configService)
    {
        if (!configService.IsDebug())
        {
            _pourService = new PourService(configService);
        }
        else
        {
            _pourService = new MockPourService();
        }

        _pourService.FlowCounterUpdated += PourService_FlowCounterUpdated;

        _updateTimer = new System.Timers.Timer(TimeSpan.FromSeconds(1)); // Sets the interval to 1000 milliseconds (1 second)
        _updateTimer.Elapsed += UpdateTimer_Elapsed;
        _updateTimer.Start();

        InitializeComponent();
    }

    private void OpenValveButton_Click(object sender, RoutedEventArgs e)
    {
        _pourService.OpenValve();
        _initialFlowCount = _pourService.FlowCounter;
    }

    private void CloseValveButton_Click(object sender, RoutedEventArgs e)
    {
        _pourService.CloseValve();
        var finalFlowCount = _pourService.FlowCounter;
        var difference = finalFlowCount - _initialFlowCount;
        if (difference > 0)
        {
            var millilitersPoured = PromptForMillilitersPoured();
            var pulsesPerMl = difference / millilitersPoured;
            PulsesPerMlTextBox.Text = pulsesPerMl.ToString("N3");
        }
    }

    private static double PromptForMillilitersPoured()
    {
        // TODO: Implement a prompt to ask the user how many milliliters were poured
        // This example just returns a hardcoded value for demonstration purposes
        return 100; // Assume 100 ml was poured for this example
    }

    private void PourService_FlowCounterUpdated(object? sender, double e)
    {
        _latestFlowCount = e; // Update the latest flow count
        if (!_updatePending)
        {
            _updatePending = true; // Set flag to indicate an update is needed
        }
    }

    private void UpdateTimer_Elapsed(object? sender, object e)
    {
        if (_updatePending)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                FlowCounterTextBlock.Text = _latestFlowCount.ToString();
            });
            _updatePending = false; // Reset update flag after handling
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
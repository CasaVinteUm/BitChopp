using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;

namespace BitChopp.Main.Views;

using System.Timers;
using Interfaces;
using Services;

public class ConfigurationWindow : KioskBaseWindow
{
    private readonly IPourService _pourService;

    private TextBox? _pulsesPerMlTextBox;
    private TextBox? _beerTypeTextBox;
    private TextBox? _beerImagePathTextBox;
    private Button? _saveButton;
    private Button? _openValveButton;
    private Button? _closeValveButton;
    private TextBlock? _flowCounterTextBlock;
    private int _initialFlowCount;

    private readonly Timer _updateTimer;
    private int _latestFlowCount;
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

        _updateTimer = new Timer(TimeSpan.FromSeconds(2)); // Sets the interval to 1000 milliseconds (1 second)
        _updateTimer.Elapsed += UpdateTimer_Elapsed;
        _updateTimer.Start();

        InitializeComponents();
    }

    private void InitializeComponents()
    {
        _pulsesPerMlTextBox = new TextBox { Width = 200 };
        _beerTypeTextBox = new TextBox { Width = 200 };
        _beerImagePathTextBox = new TextBox { Width = 200 };
        _flowCounterTextBlock = new TextBlock { Width = 200 };

        _saveButton = new Button { Content = "Save" };
        _saveButton.Click += SaveButton_Click;

        _openValveButton = new Button { Content = "Open Valve" };
        _openValveButton.Click += OpenValveButton_Click;

        _closeValveButton = new Button { Content = "Close Valve" };
        _closeValveButton.Click += CloseValveButton_Click;

        var stackPanel = new StackPanel { Orientation = Orientation.Vertical };
        stackPanel.Children.Add(new TextBlock { Text = "Pulses per mL:" });
        stackPanel.Children.Add(_pulsesPerMlTextBox);
        stackPanel.Children.Add(new TextBlock { Text = "Type of Beer:" });
        stackPanel.Children.Add(_beerTypeTextBox);
        stackPanel.Children.Add(new TextBlock { Text = "Beer Image Path:" });
        stackPanel.Children.Add(_beerImagePathTextBox);
        stackPanel.Children.Add(new TextBlock { Text = "Flow Counter:" });
        stackPanel.Children.Add(_flowCounterTextBlock);
        stackPanel.Children.Add(_openValveButton);
        stackPanel.Children.Add(_closeValveButton);
        stackPanel.Children.Add(_saveButton);

        Content = stackPanel;
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
            _pulsesPerMlTextBox.Text = pulsesPerMl.ToString("N3");
        }
    }

    private double PromptForMillilitersPoured()
    {
        // Implement a prompt to ask the user how many milliliters were poured
        // This example just returns a hardcoded value for demonstration purposes
        return 50; // Assume 100 ml was poured for this example
    }

    private void PourService_FlowCounterUpdated(object? sender, int e)
    {
        _latestFlowCount = e; // Update the latest flow count
        if (!_updatePending)
        {
            _updatePending = true; // Set flag to indicate an update is needed
        }
    }

    private void UpdateTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        if (_updatePending)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                _flowCounterTextBlock.Text = _latestFlowCount.ToString();
            });
            _updatePending = false; // Reset update flag after handling
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var settings = new
        {
            PulsesPerMl = _pulsesPerMlTextBox.Text,
            BeerType = _beerTypeTextBox.Text,
            BeerImagePath = _beerImagePathTextBox.Text
        };

        // File.WriteAllText("appsettings.json", JsonConverter.SerializeObject(settings));
        Close();
    }
}
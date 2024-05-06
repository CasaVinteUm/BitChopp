using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace BitChopp.Main.Views;

using Models;
using Services;
using ViewModels;

public partial class MainWindow : KioskBaseWindow
{
    private readonly DispatcherTimer _resetTimer;

    private int _logoClickCount = 0;

    // This constructor is used by Avalonia
#pragma warning disable CS8625
    public MainWindow() : base(null)
    {
        InitializeComponent();
        _resetTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
    }
#pragma warning restore CS8625
    public MainWindow(ConfigService configService) : base(configService)
    {
        InitializeComponent();

        // Setup the timer
        _resetTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
        _resetTimer.Tick += (s, e) =>
        {
            _logoClickCount = 0;  // Reset count after 10 seconds
            _resetTimer.Stop();
        };

        Logo.PointerPressed += (s, e) =>
        {
            _logoClickCount++;

            if (_logoClickCount == 1)  // Start the timer on the first click
            {
                _resetTimer.Start();
            }

            if (_logoClickCount >= 10)
            {
                _resetTimer.Stop();
                _logoClickCount = 0; // Reset count
                OpenConfigurationWindow();
            }
        };
    }

    private void OnSwitchButtonClick(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        if (button?.DataContext is LnUrlPosSwitch switchItem)
        {
            var vm = DataContext as MainViewModel;

            vm?.SwitchCommand.Execute(new SwitchCommandObject(this, switchItem));
        }
    }

    private void OpenConfigurationWindow()
    {
        var configWindow = new ConfigurationWindow(ConfigService)
        {
            Topmost = true
        };
        configWindow.ShowDialog(this);
    }
}

using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Threading;
using DynamicData;
using ReactiveUI;

namespace BitChopp.Main.ViewModels;

using Interfaces;
using Models;
using Services;
using Views;

public partial class MainViewModel : ReactiveObject
{
    [GeneratedRegex(@"(\d+)(ml|L)")]
    private static partial Regex VolumeRegex();

    private readonly ConfigService _configService;
    private readonly IPourService _pourService;

    private bool _isLoading;
    private SuccessViewModel? _successViewModel;

    public readonly string DeviceId;

    public ObservableCollection<LnUrlPosSwitch> Switches { get; } = [];
    public ICommand SwitchCommand { get; }
    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    public MainViewModel(ApiService apiService, ConfigService configService)
    {
        SwitchCommand = ReactiveCommand.Create<SwitchCommandObject>(OnSwitchSelected);

        DeviceId = configService.GetSwitchId();
        _ = LoadSwitchesAsync(apiService);

        _configService = configService;

        if (!_configService.IsDebug())
        {
            _pourService = new PourService(configService);
        }
        else
        {
            _pourService = new MockPourService();
        }
    }

    private async void OnSwitchSelected(SwitchCommandObject swObj)
    {
        if (swObj.Switch == null || string.IsNullOrEmpty(swObj.Switch.Lnurl))
        {
            return;
        }

        // Logic to handle switch selection, such as navigating to a new screen and displaying the QR code
        Console.WriteLine($"Selected Switch's Lnurl: {swObj.Switch.Lnurl}");

        var qrWindow = new QRCodeWindow(DeviceId, swObj.Switch.Pin, swObj.Switch.Lnurl, _configService)
        {
            Topmost = true // Make the window always on top
        };
        await qrWindow.ShowDialog(swObj.Window);

        if (qrWindow.WebSocketResult == "Paid" || _configService.IsDebug())
        {
            await ShowSuccessWindow(swObj);
        }

        qrWindow.Close();
        qrWindow = null;
    }

    private async Task ShowSuccessWindow(SwitchCommandObject swObj)
    {
        var volume = ExtractVolume(swObj.Switch.Description);
        _successViewModel = new SuccessViewModel(volume, _configService);

        _pourService.FlowCounterUpdated += (s, e) =>
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                _successViewModel.FlowCounter = e;
            });
        };

        _pourService.PourEnded += (s, e) =>
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                _successViewModel.PourEnded = e;
                Console.WriteLine("MainView:PourEnded event received");
            });
        };

        Console.WriteLine($"Pouring {volume} ml");
        _ = Dispatcher.UIThread.InvokeAsync(() =>
        {
            _pourService.PourExactly(volume);
        });

        Console.WriteLine("Showing success window");
        var successWindow = new SuccessWindow(_successViewModel)
        {
            Topmost = true
        };
        await successWindow.ShowDialog(swObj.Window);
        Console.WriteLine("Success window closed");
        successWindow.Close();
        successWindow = null;
    }

    private async Task LoadSwitchesAsync(ApiService apiService)
    {
        IsLoading = true;

        var lnUrlPosDevices = await apiService.FetchLnurlPos() ?? throw new Exception("Failed to fetch data");

        if (lnUrlPosDevices.Count == 0)
        {
            Console.Error.WriteLine("Failed to fetch a valid list of lnurl devices");
            return;
        }

        var lnUrlDevice = lnUrlPosDevices.FirstOrDefault(r => r.Id?.ToString() == DeviceId) ?? throw new Exception("Could not find the device with the specified ID");

        UpdateUI(lnUrlDevice.Switches);

        IsLoading = false;
    }

    private static int ExtractVolume(string text)
    {
        // This regex pattern looks for numbers followed directly by either 'ml' or 'L'
        var match = VolumeRegex().Match(text);

        if (match.Success)
        {
            // Extract the numeric part of the match
            var number = int.Parse(match.Groups[1].Value);

            // Check if the unit is 'L' and convert to milliliters if necessary
            if (match.Groups[2].Value == "L")
            {
                return number * 1000;
            }
            else // It's already in 'ml', so return as is
            {
                return number;
            }
        }
        else
        {
            throw new InvalidOperationException("No volume information found in the string.");
        }
    }

    private void UpdateUI(List<LnUrlPosSwitch> switches)
    {
        // Ensure UI updates happen on the UI thread
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            Switches.Clear();
            Switches.AddRange([.. switches.OrderBy(x => x.Amount)]);
        });
    }
}

public class SwitchCommandObject(Window window, LnUrlPosSwitch switchItem)
{
    public Window Window { get; set; } = window;
    public LnUrlPosSwitch Switch { get; set; } = switchItem;
}

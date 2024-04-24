using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using DynamicData;
using ReactiveUI;

namespace BitChopp.Main.ViewModels;

using Models;
using Services;
using Views;

public class MainViewModel : ReactiveObject
{
    public ObservableCollection<LnUrlPosSwitch> Switches { get; } = [];

    public readonly string DeviceId;
    private readonly ConfigService _configService;
    private readonly PourService _pourService;

    public ICommand SwitchCommand { get; }

    public MainViewModel(ApiService apiService, ConfigService configService)
    {
        SwitchCommand = ReactiveCommand.Create<SwitchCommandObject>(OnSwitchSelected);

        DeviceId = configService.GetSwitchId();
        _ = LoadSwitchesAsync(apiService);

        _configService = configService;
        _pourService = new PourService(configService);
    }

    private async void OnSwitchSelected(SwitchCommandObject swObj)
    {
        if (swObj.Switch == null || string.IsNullOrEmpty(swObj.Switch.Lnurl))
        {
            return;
        }

        // Logic to handle switch selection, such as navigating to a new screen and displaying the QR code
        Console.WriteLine($"Selected Switch's Lnurl: {swObj.Switch.Lnurl}");

        var qrWindow = new QRCodeWindow(DeviceId, swObj.Switch.Pin, swObj.Switch.Lnurl, _configService);
        var task = qrWindow.ShowDialog(swObj.Window);
        qrWindow.Topmost = true; // Make the window always on top
        await task;

        var result = qrWindow.WebSocketResult;

        _pourService.PourExactly(100);
    }

    private async Task LoadSwitchesAsync(ApiService apiService)
    {
        var lnUrlPosDevices = (await apiService.FetchLnurlPos()) ?? throw new Exception("Failed to fetch data");

        if (lnUrlPosDevices.Count == 0)
        {
            Console.Error.WriteLine("Failed to fetch a valid list of lnurl devices");
            return;
        }

        var lnUrlDevice = lnUrlPosDevices.FirstOrDefault(r => r.Id?.ToString() == DeviceId) ?? throw new Exception("Could not find the device with the specified ID");

        UpdateUI(lnUrlDevice.Switches);
    }

    private void UpdateUI(List<LnUrlPosSwitch> switches)
    {
        // Ensure UI updates happen on the UI thread
        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
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

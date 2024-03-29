using Avalonia.Controls;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BitChopp.ViewModels;

using Models;
using Views;

public class MainViewModel : ReactiveObject
{
    public ObservableCollection<LnUrlPosSwitch> Switches { get; } = [];

    public readonly string DeviceId;

    public ICommand SwitchCommand { get; }

    public MainViewModel(ApiService apiService, ConfigService configService)
    {
        SwitchCommand = ReactiveCommand.Create<SwitchCommandObject>(OnSwitchSelected);

        DeviceId = configService.GetSwitchId();
        _ = LoadSwitchesAsync(apiService);
    }

    private async void OnSwitchSelected(SwitchCommandObject swObj)
    {
        if (swObj.Switch == null || string.IsNullOrEmpty(swObj.Switch.Lnurl))
        {
            return;
        }

        // Logic to handle switch selection, such as navigating to a new screen and displaying the QR code
        Console.WriteLine($"Selected Switch's Lnurl: {swObj.Switch.Lnurl}");

        var qrWindow = new QRCodeWindow();
        qrWindow.SetData(DeviceId, swObj.Switch.Lnurl); // Assuming switch.Lnurl contains the URL for the QR code
        var task = qrWindow.ShowDialog(swObj.Window);
        qrWindow.Topmost = true; // Make the window always on top
        await task;

        var result = qrWindow.WebSocketResult;
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

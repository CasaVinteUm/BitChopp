using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;

namespace BitChopp.ViewModels;

using Views;

public class MainViewModel : ReactiveObject
{
    public ObservableCollection<Switch> Switches { get; } = [];
    public string? DeviceId { get; private set; }

    public ICommand SwitchCommand { get; }

    public MainViewModel(ApiService apiService)
    {
        SwitchCommand = ReactiveCommand.Create<SwitchComandObject>(OnSwitchSelected);

        _ = LoadSwitchesAsync(apiService);
    }

    private async void OnSwitchSelected(SwitchComandObject swObj)
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
        var responseItems = await apiService.FetchDataAsync() ?? throw new Exception("Failed to fetch data");
        if (responseItems?.Count == 0 || responseItems?[0] == null || responseItems[0].Switches?.Count == 0)
        {
            // Handle error
            return;
        }

        DeviceId = responseItems[0].Id;

        UpdateUI(responseItems[0].Switches!);
    }

    private void UpdateUI(List<Switch> switches)
    {
        // Ensure UI updates happen on the UI thread
        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            Switches.Clear();
            switches = [.. switches.OrderBy(x => x.Amount)];
            for (var i = 0; i < switches.Count; i++)
            {
                switches[i].Description = i switch
                {
                    0 => "Half Pint (284ml)",
                    1 => "Pint (568ml)",
                    2 => "Oktoberfest (1L)",
                    _ => "Unknown",
                };
                Switches.Add(switches[i]);
            }
            // Trigger any other UI updates or notifications here
        });
    }
}

public class SwitchComandObject(Window window, Switch switchItem)
{
    public Window Window { get; set; } = window;
    public Switch Switch { get; set; } = switchItem;
}
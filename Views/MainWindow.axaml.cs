using Avalonia.Controls;
using Avalonia.Interactivity;

using BitChopp.Models;
using BitChopp.ViewModels;

namespace BitChopp.Views;

public partial class MainWindow : Window
{
    public MainWindow(ConfigService configService)
    {
        InitializeComponent();

        var isKiosk = configService.IsKiosk();
        if (isKiosk) {
            WindowState = WindowState.FullScreen;
            ExtendClientAreaToDecorationsHint = true;
            ExtendClientAreaTitleBarHeightHint = -1d;
            SystemDecorations = SystemDecorations.None;
            ShowInTaskbar = false;
            Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.None);
        }
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
}

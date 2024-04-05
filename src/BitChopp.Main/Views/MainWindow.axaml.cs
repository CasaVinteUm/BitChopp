using Avalonia.Controls;
using Avalonia.Interactivity;

namespace BitChopp.Main.Views;

using Models;
using Services;
using ViewModels;

public partial class MainWindow : KioskBaseWindow
{
    public MainWindow(ConfigService configService) : base(configService)
    {
        InitializeComponent();
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

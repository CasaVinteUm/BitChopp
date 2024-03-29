using Avalonia.Controls;
using Avalonia.Interactivity;
using BitChopp.ViewModels;

namespace BitChopp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Hide the cursor for this window
        Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.None);
    }

    private void OnSwitchButtonClick(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        if (button?.DataContext is Switch switchItem)
        {
            var vm = DataContext as MainViewModel;
            vm?.SwitchCommand.Execute(new SwitchComandObject(this, switchItem));
        }
    }
}
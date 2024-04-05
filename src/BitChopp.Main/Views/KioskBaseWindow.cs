using Avalonia.Controls;
using Avalonia.Input;

namespace BitChopp.Main.Views;

using Services;

public partial class KioskBaseWindow : Window
{
    public KioskBaseWindow(ConfigService configService)
    {
        if (configService.IsKiosk())
        {
            WindowState = WindowState.FullScreen;
            ExtendClientAreaToDecorationsHint = true;
            ExtendClientAreaTitleBarHeightHint = -1d;
            SystemDecorations = SystemDecorations.None;
            ShowInTaskbar = false;
            Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.None);
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        // Prevent Alt+F4 from closing the application, etc.
        if (e.Key == Key.F4 && e.KeyModifiers == KeyModifiers.Alt)
        {
            e.Handled = true;
        }
        else
        {
            base.OnKeyDown(e);
        }
    }
}
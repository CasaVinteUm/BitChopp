using Avalonia.Threading;

namespace BitChopp.Main.Views;

using Services;

public partial class SuccessWindow : KioskBaseWindow
{
    private readonly PouringTipsService _pouringTipsService;
    private readonly Timer _fakeBeerFlowTimer;
    private readonly Timer _countDownTimer;

    private int _timeLeft = 60; // Seconds
    private string _timerMessage = "Tire seu Chopp em até {0} segundos...";

    // This constructor is used by Avalonia
#pragma warning disable CS8625
    public SuccessWindow() : base(null)
    {
        _pouringTipsService = new PouringTipsService(0);
        _countDownTimer = new Timer(null, null, Timeout.Infinite, Timeout.Infinite);
        _fakeBeerFlowTimer = new Timer(null, null, Timeout.Infinite, Timeout.Infinite);
    }
#pragma warning restore CS8625
    public SuccessWindow(int volume, ConfigService configService) : base(configService)
    {
        InitializeComponent();

        _pouringTipsService = new PouringTipsService(volume);
        _countDownTimer = new Timer(UpdateCountdown, null, TimeSpan.Zero, TimeSpan.FromSeconds(1)); // Tick every second

        beerProgressBar.Maximum = volume;
        _fakeBeerFlowTimer = new Timer(UpdateBeerProgressBar, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100)); // Tick every second

        Closing += (sender, e) =>
        {
            _countDownTimer.Dispose();
            _fakeBeerFlowTimer.Dispose();
        };
    }

    private void UpdateCountdown(object? state)
    {
        _timeLeft--;

        // Update UI on the UI thread
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            try
            {
                timerTextBlock.Text = string.Format(_timerMessage, _timeLeft);

                if (_timeLeft <= 0)
                {
                    _countDownTimer.Dispose();
                    _fakeBeerFlowTimer.Dispose();
                    Close();
                }
            }
            catch
            {
                // Ignore
            }
        });
    }

    // Placeholder for actual beer flow sensor handling
    private void UpdateBeerProgressBar(object? state)
    {
        // Update UI on the UI thread
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            try
            {
                beerProgressBar.Value += 50;

                _pouringTipsService.UpdateTipByVolume(tipTextBlock, (int)beerProgressBar.Value);

                if (beerProgressBar.Value >= beerProgressBar.Maximum)
                {
                    _fakeBeerFlowTimer.Dispose();
                    ShowCheers();
                }
            }
            catch
            {
                // Ignore
            }
        });
    }

    private void ShowCheers()
    {
        _timerMessage = "Fechando em {0} segundos...";
        _timeLeft = 5;
        _ = new DispatcherTimer(TimeSpan.FromSeconds(5), DispatcherPriority.Normal, (sender, e) =>
        {
            Close();
        });

        Dispatcher.UIThread.InvokeAsync(() =>
        {
            try
            {
                _pouringTipsService.UpdateTipByVolume(tipTextBlock, -1);

                beerProgressBar.IsVisible = false;
                cheersAnimation.IsVisible = true;
            }
            catch
            {
                // Ignore
            }
        });
    }
}
using Avalonia;
using Avalonia.Controls;
using System;
using System.Threading;
using Avalonia.Threading;

namespace BitChopp.Main.Views;

using Services;

public partial class SuccessWindow : KioskBaseWindow
{
    private readonly PouringTipsService _pouringTipsService;
    private readonly ProgressBar _beerProgressBar;
    private readonly TextBlock _timerTextBlock;
    private readonly TextBlock _tipTextBlock;
    private readonly Timer _countDownTimer;
    private readonly Timer _fakeBeerFlowTimer;

    private int _timeLeft = 60; // Seconds

    // This constructor is used by Avalonia
#pragma warning disable CS8625
    public SuccessWindow() : base(null)
    {
        _pouringTipsService = new PouringTipsService(0);
        _beerProgressBar = new ProgressBar();
        _timerTextBlock = new TextBlock();
        _tipTextBlock = new TextBlock();
        _countDownTimer = new Timer(null, null, Timeout.Infinite, Timeout.Infinite);
        _fakeBeerFlowTimer = new Timer(null, null, Timeout.Infinite, Timeout.Infinite);
    }
#pragma warning restore CS8625
    public SuccessWindow(int volume, ConfigService configService) : base(configService)
    {
        InitializeComponent();

#if DEBUG
        this.AttachDevTools();
#endif

        _pouringTipsService = new PouringTipsService(volume);

        _beerProgressBar = this.FindControl<ProgressBar>("beerProgressBar") ?? throw new NullReferenceException("beerProgressBar not found");
        _timerTextBlock = this.FindControl<TextBlock>("timerTextBlock") ?? throw new NullReferenceException("timerTextBlock not found");
        _tipTextBlock = this.FindControl<TextBlock>("tipTextBlock") ?? throw new NullReferenceException("tipTextBlock not found");
        _countDownTimer = new Timer(UpdateCountdown, null, TimeSpan.Zero, TimeSpan.FromSeconds(1)); // Tick every second

        _beerProgressBar.Maximum = volume;
        _fakeBeerFlowTimer = new Timer(UpdateBeerProgressBar, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100)); // Tick every second
    }

    private void UpdateCountdown(object? state)
    {
        _timeLeft--;

        // Update UI on the UI thread
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            timerTextBlock.Text = $"Tire seu Chopp em até {_timeLeft} segundos...";
            // _beerProgressBar.Value = (1 - (_timeLeft / 60.0)) * 100;

            if (_timeLeft <= 0)
            {
                _countDownTimer.Dispose();
                Close();
            }
        });
    }

    // Placeholder for actual beer flow sensor handling
    private void UpdateBeerProgressBar(object? state)
    {
        // Update UI on the UI thread
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            _beerProgressBar.Value += 10;

            _pouringTipsService.UpdateTipByVolume(_tipTextBlock, (int)_beerProgressBar.Value);

            if (_beerProgressBar.Value >= _beerProgressBar.Maximum)
            {
                _fakeBeerFlowTimer.Dispose();
                Close();
            }
        });
    }
}
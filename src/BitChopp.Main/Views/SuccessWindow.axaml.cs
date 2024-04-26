using Avalonia.Threading;
using ReactiveUI;

namespace BitChopp.Main.Views;

using Services;
using ViewModels;

public partial class SuccessWindow : KioskBaseWindow
{
    private readonly PouringTipsService _pouringTipsService;
    private readonly Timer _countDownTimer;
    private readonly SuccessViewModel _viewModel;

    private int _timeLeft = 60; // Seconds
    private string _timerMessage = "Tire seu Chopp em até {0} segundos...";

    // This constructor is used by Avalonia
#pragma warning disable CS8625
    public SuccessWindow() : base(null)
    {
        _pouringTipsService = new PouringTipsService(0);
        _countDownTimer = new Timer(null, null, Timeout.Infinite, Timeout.Infinite);
        _viewModel = new SuccessViewModel(0, null);
    }
#pragma warning restore CS8625
    public SuccessWindow(SuccessViewModel viewModel) : base(viewModel.ConfigService)
    {
        Console.WriteLine("SuccessWindow constructor");

        DataContext = viewModel;
        _viewModel = viewModel;

        _pouringTipsService = new PouringTipsService(viewModel.Volume);
        _countDownTimer = new Timer(UpdateCountdown, null, TimeSpan.Zero, TimeSpan.FromSeconds(1)); // Tick every second

        Activated += (sender, e) =>
        {
            this.WhenAnyValue(x => x._viewModel.PourEnded, x => x._viewModel.FlowCounter)
                .Subscribe(_ => HandleViewModelUpdates());
        };

        Closing += (sender, e) =>
        {
            _countDownTimer.Dispose();
        };

        InitializeComponent();

        HandleViewModelUpdates();
    }

    private void HandleViewModelUpdates()
    {
        Console.WriteLine("HandleViewModelUpdates");
        try
        {
            if (_viewModel.PourEnded)
            {
                Console.WriteLine("Success:PourEnded");
                ShowCheers();
            }

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                try
                {
                    _pouringTipsService.UpdateTipByVolume(tipTextBlock, _viewModel.FlowCounter);
                }
                catch
                {
                    // Ignore
                }
            });
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            Console.Error.WriteLine(ex.StackTrace);
        }
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
                    Close();
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
        Console.WriteLine("ShowCheers");
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

    ~SuccessWindow()
    {
        Console.WriteLine("SuccessWindow destructor");
    }
}
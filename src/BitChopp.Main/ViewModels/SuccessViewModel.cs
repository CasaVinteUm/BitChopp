using ReactiveUI;

namespace BitChopp.Main.ViewModels;

using Services;

public class SuccessViewModel : ReactiveObject
{
    private double _flowCounter;
    private int _volume;
    private bool _pourEnded;

    public readonly ConfigService ConfigService;
    public double FlowCounter
    {
        get => _flowCounter;
        set => this.RaiseAndSetIfChanged(ref _flowCounter, value);
    }

    public int Volume
    {
        get => _volume;
        set => this.RaiseAndSetIfChanged(ref _volume, value);
    }

    public bool PourEnded
    {
        get => _pourEnded;
        set => this.RaiseAndSetIfChanged(ref _pourEnded, value);
    }

    public SuccessViewModel(int volume, ConfigService configService)
    {
        ConfigService = configService;
        Volume = volume;
    }
}
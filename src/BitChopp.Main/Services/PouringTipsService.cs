
using Avalonia.Controls;

namespace BitChopp.Main.Services;
public class PouringTipsService(int maxVolume)
{
    private const string TipStart = "Puxe a alavanca para começar a servir.";
    private const string TipTiltGlass = "Incline o copo em 45° para evitar espuma.";
    private const string TipHangTight = "Calma, você já vai tomar seu chopp...";
    private const string TipKeepTilted = "Continue inclinando o copo para evitar espuma.";
    private const string TipFinishPour = "Agora deixe o copo nivelado e empurre a alavanca para finalizar o chopp com um belo colarinho.";
    private const string TipCheers = "Saúde!";
    private const int VolumeForTilt = 10; // volume in ml when we tell the user to tilt the glass
    private const int PercentageForFinish = 90;

    private readonly int _volumeForFinish = (int)(maxVolume * (PercentageForFinish / 100.0));
    private readonly int _volumeForTipChange = maxVolume / 5;

    private string _currentTip = TipStart;

    // Call this method whenever there's an update from the sensor
    public void UpdateTipByVolume(TextBlock textBlock, int volume)
    {
        var lastTip = _currentTip;

        if (volume == -1)
        {
            _currentTip = TipCheers;
        }
        // Tilt glass tip should be shown once when the pouring starts
        else if (volume >= VolumeForTilt && volume < _volumeForTipChange)
        {
            _currentTip = TipTiltGlass;
        }
        // Special instruction to keep the glass tilted every 10% of the pour
        else if (volume % _volumeForTipChange < VolumeForTilt && volume < _volumeForFinish)
        {
            _currentTip = TipHangTight;
        }
        // General encouragement message for other times
        else if (volume >= _volumeForTipChange && volume < _volumeForFinish)
        {
            _currentTip = TipKeepTilted;
        }
        // Finish pouring
        else if (volume >= _volumeForFinish)
        {
            _currentTip = TipFinishPour;
        }

        if (_currentTip != lastTip)
        {
            textBlock.Text = _currentTip;
            AdjustFontSizeForText(textBlock);
        }
    }

    private static void AdjustFontSizeForText(TextBlock textBlock)
    {
        var size = textBlock.Text!.Length * textBlock.FontSize;

        if (size < 1200)
        {
            while (size < 1200 && textBlock.FontSize < 75)
            {
                textBlock.FontSize += 1;
                size = textBlock.Text!.Length * textBlock.FontSize;
            }
        }
        else
        {
            while (size > 1200 && textBlock.FontSize > 25)
            {
                textBlock.FontSize -= 1;
                size = textBlock.Text!.Length * textBlock.FontSize;
            }
        }
    }
}
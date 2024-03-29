using System.IO;
using Avalonia.Controls;
using QRCoder;

namespace BitChopp.Views;

public partial class QRCodeWindow : Window
{
    public QRCodeWindow()
    {
        InitializeComponent();
    }

    public void SetQRCode(string url)
    {
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.L);
        var qrCode = new PngByteQRCode(qrCodeData);

        var qrCodeImage = qrCode.GetGraphic(20);
        var ms = new MemoryStream(qrCodeImage);

        var bitmap = new Avalonia.Media.Imaging.Bitmap(ms);
        QrCodeImage.Source = bitmap;
    }

    private void Close(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
}
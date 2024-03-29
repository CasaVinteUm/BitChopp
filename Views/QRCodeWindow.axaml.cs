using Avalonia.Controls;
using Avalonia.Threading;
using QRCoder;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace BitChopp.Views;

public partial class QRCodeWindow : Window
{
    private readonly ClientWebSocket _webSocket = new();

    private string? _deviceId;

    public string? WebSocketResult { get; private set; }

    public QRCodeWindow()
    {
        InitializeComponent();

        OpenWebSocketAsync();
    }

    public void SetData(string id, string url)
    {
        _deviceId = id;

        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.L);
        var qrCode = new PngByteQRCode(qrCodeData);

        var qrCodeImage = qrCode.GetGraphic(20);
        var ms = new MemoryStream(qrCodeImage);

        var bitmap = new Avalonia.Media.Imaging.Bitmap(ms);
        QrCodeImage.Source = bitmap;
    }

    private void CloseDialog(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Dispatcher.UIThread.Post(() => Close());
    }

    private async void OpenWebSocketAsync()
    {
        try
        {
            await _webSocket.ConnectAsync(new Uri($"wss://lnbits.casa21.space/api/v1/ws/{_deviceId}"), CancellationToken.None);
            StartListening();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket connection error: {ex.Message}");
        }
    }

    private async void StartListening()
    {
        var buffer = new byte[1024 * 4];
        while (_webSocket.State == WebSocketState.Open)
        {
            var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                // Handle the message
                WebSocketResult = message;

                Close();

                break; // Close after receiving the first message
            }
        }
    }
}

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

    private readonly string _deviceId;
    private readonly int _pinId;
    private readonly string _lnUrl;

    public string? WebSocketResult { get; private set; }

    public QRCodeWindow(string deviceId, int pinId, string lnUrl, ConfigService configService)
    {
        InitializeComponent();

        _deviceId = deviceId;
        _pinId = pinId;
        _lnUrl = lnUrl;

        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(_lnUrl, QRCodeGenerator.ECCLevel.L);
        var qrCode = new PngByteQRCode(qrCodeData);

        var qrCodeImage = qrCode.GetGraphic(20);
        var ms = new MemoryStream(qrCodeImage);

        var bitmap = new Avalonia.Media.Imaging.Bitmap(ms);
        QrCodeImage.Source = bitmap;

        var isKiosk = configService.IsKiosk();
        if (isKiosk) {
            WindowState = WindowState.FullScreen;
            ExtendClientAreaToDecorationsHint = true;
            ExtendClientAreaTitleBarHeightHint = -1d;
            SystemDecorations = SystemDecorations.None;
            ShowInTaskbar = false;
            Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.None);
        }

        Dispatcher.UIThread.InvokeAsync(() => OpenWebSocketAsync());
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
            if (result.MessageType != WebSocketMessageType.Text) {
                Console.WriteLine("Received non-text WebSocket message.");
                continue;
            }

            var message = Encoding.UTF8.GetString(buffer, 0, result.Count).Split('-');

            var pin = int.Parse(message[0]);
            var duration = int.Parse(message[1]);

            // TODO: Check PIN id; message = pinId-duration
            if(pin != _pinId) {
                Console.Error.WriteLine("Invalid PIN. Someone probably paid old invoice");
                return;
            }

            WebSocketResult = "Paid";

            _ = _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);

            Close();
            return;
        }
    }
}

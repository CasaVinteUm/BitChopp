using System.Net.WebSockets;
using System.Text;
using Avalonia.Threading;
using QRCoder;

namespace BitChopp.Main.Views;

using Services;

public partial class QRCodeWindow : KioskBaseWindow
{
    private readonly ClientWebSocket _webSocket = new();

    private readonly string _deviceId;
    private readonly int _pinId;
    private readonly string _lnUrl;
    private readonly Uri _wsUrl;

    public string? WebSocketResult { get; private set; }

    // This constructor is used by Avalonia
#pragma warning disable CS8625
    public QRCodeWindow() : base(null)
    {
        InitializeComponent();
        _deviceId = string.Empty;
        _lnUrl = string.Empty;
    }
#pragma warning restore CS8625
    public QRCodeWindow(string deviceId, int pinId, string lnUrl, ConfigService configService) : base(configService)
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

        _wsUrl = configService.GetWsHost();

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
            await _webSocket.ConnectAsync(_wsUrl, CancellationToken.None);
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
            if (result.MessageType != WebSocketMessageType.Text)
            {
                if (result.MessageType != WebSocketMessageType.Text)
                {
                    Console.WriteLine("Received non-text WebSocket message.");
                    continue;
                }

                var message = Encoding.UTF8.GetString(buffer, 0, result.Count).Split('-');

                var pin = int.Parse(message[0]);
                var duration = int.Parse(message[1]);

                // TODO: Check PIN id; message = pinId-duration
                if (pin != _pinId)
                {
                    if (pin != _pinId)
                    {
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
    }
}
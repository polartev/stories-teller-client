using CommunityToolkit.Mvvm.ComponentModel;
using Story_Teller.IServices;
using System.Net.WebSockets;
using System.Text;

namespace Story_Teller.Services;

public partial class WebSocketService : ObservableObject, IWebSocketService
{
    public string ServiceName => "WebSocketService";

    [ObservableProperty]
    private bool? isOnline = null;

    private ClientWebSocket? socket;
    private CancellationTokenSource? cts;

    public event Action<string>? OnMessageReceived;

    IAlertService alertService;
    IUserService userService;

    public WebSocketService(IAlertService alertService, IUserService userService)
    {
        this.alertService = alertService;
        this.userService = userService;
    }

    public async Task ConnectAsync()
    {
        socket = new ClientWebSocket();
        cts = new CancellationTokenSource();

        try
        {
            var uri = new Uri($"wss://api.stories-teller.com/ws/user/{userService.User.Name}");
            await socket.ConnectAsync(uri, cts.Token);

            if (socket.State == WebSocketState.Open)
            {
                _ = ReceiveLoop();
                IsOnline = true;
            }
            else
            {
                IsOnline = false;
            }
        }
        catch (Exception ex)
        {
            IsOnline = false;
#if DEBUG
            await alertService.ShowAlertAsync("Debug", $"Connection failed: {ex.Message}");
#endif
        }
    }

    private async Task ReceiveLoop()
    {
        if (socket == null || cts == null)
        {
            return;
        }

        var buffer = new byte[4096];

        try
        {
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    IsOnline = false;
                    break;
                }
                else
                {
                    var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    OnMessageReceived?.Invoke(msg);
                }
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            await alertService.ShowAlertAsync("Error", $"WebSocket error: {ex.Message}");
#endif
        }
        finally
        {
            IsOnline = false;
        }
    }

    public async Task SendAsync(string message)
    {
        if (socket == null || socket.State != WebSocketState.Open || cts == null)
        {
            throw new InvalidOperationException("WebSocket is not connected.");
        }

        var bytes = Encoding.UTF8.GetBytes(message);
        await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cts.Token);
    }

    public async Task DisconnectAsync()
    {
        if (socket != null && socket.State == WebSocketState.Open && cts != null)
        {
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnect", cts.Token);
        }

        cts?.Cancel();
        socket?.Dispose();
        IsOnline = false;
    }
}
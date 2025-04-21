using CommunityToolkit.Mvvm.ComponentModel;
using Story_Teller.IServices;
using System.Net.WebSockets;
using System.Text;

namespace Story_Teller.Services;

public partial class WebSocketService : ObservableObject, IWebSocketService
{
    public string ServiceName => "WebSocketService";

    [ObservableProperty]
    private bool isOnline = false;

    private ClientWebSocket? socket;
    private CancellationTokenSource? cts;
    private readonly string userId;
    public event Action<string>? OnMessageReceived;

    public WebSocketService()
    {
        // DELETE THESE LINES!!!! JUST FOR DEBUGGING!!! IMPLEMENT USER CLASSES AND DATABASE LATER
        userId = "artem";
        // DELETE THESE LINES!!!! JUST FOR DEBUGGING!!! IMPLEMENT USER CLASSES AND DATABASE LATER
    }

    public async Task ConnectAsync()
    {
        socket = new ClientWebSocket();
        cts = new CancellationTokenSource();

        try
        {
            var uri = new Uri($"wss://api.stories-teller.com/ws/user/{userId}");
            await socket.ConnectAsync(uri, cts.Token);
            _ = ReceiveLoop();
            IsOnline = true;
        }
        catch
        {
            IsOnline = false;
        }
    }

    private async Task ReceiveLoop()
    {
        if (socket == null || cts == null)
            return;

        var buffer = new byte[4096];

        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
            else
            {
                var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                OnMessageReceived?.Invoke(msg);
            }
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
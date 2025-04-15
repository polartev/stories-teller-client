using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Story_Teller.Services;

public class WebSocketService
{
    private ClientWebSocket? socket;
    private CancellationTokenSource? cts;
    private readonly string userId;
    public event Action<string>? OnMessageReceived;

    public WebSocketService(string userId)
    {
        this.userId = userId;
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
        }
        catch (Exception ex)
        {
            Console.WriteLine("WebSocket connection error: " + ex.Message);
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
    }
}
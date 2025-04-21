using System;
using System.Net.WebSockets;

namespace Story_Teller.IServices;

public interface IWebSocketService : IOnlineService
{
    /// <summary>
    /// The method to be called when a message is received from the WebSocket server.
    /// </summary>
    event Action<string> OnMessageReceived;

    /// <summary>
    /// The method to send a message to the WebSocket server.
    /// </summary>
    Task SendAsync(string message);
}

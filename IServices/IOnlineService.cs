namespace Story_Teller.IServices;

public interface IOnlineService
{
    /// <summary>
    /// The name of the service, used for identification purposes.
    /// </summary>
    string ServiceName { get; }

    /// <summary>
    /// Indicates whether the service is currently online or offline.
    /// </summary>
    bool? IsOnline { get; set; }

    /// <summary>
    /// The method to establish a connection to the WebSocket server.
    /// </summary>
    Task ConnectAsync();

    /// <summary>
    /// The method to close the connection to the WebSocket server.
    /// </summary>
    Task DisconnectAsync();
}

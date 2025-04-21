using System.Collections.ObjectModel;
using System.Net.WebSockets;

namespace Story_Teller.IServices;

public interface IConnectionService
{
    /// <summary>
    /// The collection of online services that are being monitored.
    /// </summary>
    ObservableCollection<IOnlineService> Services { get; set; }

    /// <summary>
    /// Starts monitoring the connection status of a specific online service.
    /// </summary>
    /// <param name="onlineService"></param>
    void StartMonitoring(IOnlineService onlineService);

    /// <summary>
    /// Stops monitoring the connection status of a specific online service.
    /// </summary>
    /// <param name="onlineService"></param>
    void StopMonitoring(IOnlineService onlineService);
}

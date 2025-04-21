using CommunityToolkit.Mvvm.ComponentModel;
using Story_Teller.IServices;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Story_Teller.Services;

public class ConnectionService : ObservableObject, IConnectionService
{
    public ObservableCollection<IOnlineService> Services { get; set; } = new();

    /// <summary>
    /// Service for displaying alerts and notifications.
    /// </summary>
    private IAlertService alertService;

    /// <summary>
    /// Constructor for the ConnectionService class.
    /// </summary>
    /// <param name="alertService"></param>
    public ConnectionService(IAlertService alertService)
    {
        this.alertService = alertService;
    }

    public void StartMonitoring(IOnlineService service)
    {
        if (service == null)
        {
#if DEBUG
            alertService.ShowAlertAsync("Debug", $"{service?.ServiceName} is null. Check initialization. (MainViewModel.cs:30)");
#else
            alertService.ShowAlertAsync("Error", "Core components are missing. Please restart the app.");
#endif
            return;
        }

        if (!Services.Contains(service))
        {
            Services.Add(service);

            if (service is ObservableObject observable)
            {
                observable.PropertyChanged += OnServicePropertyChanged;
            }

            _ = service.ConnectAsync();
        }
        else
        {
#if DEBUG
            alertService.ShowAlertAsync("Info", $"{service?.ServiceName} is already being monitored.");
#endif
            return;
        }
    }

    public void StopMonitoring(IOnlineService service)
    {
        if (Services.Contains(service))
        {
            Services.Remove(service);

            if (service is ObservableObject observable)
            {
                observable.PropertyChanged -= OnServicePropertyChanged;
            }
        }
    }

    /// <summary>
    /// Handles the PropertyChanged event for online services.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnServicePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is IOnlineService service && e.PropertyName == nameof(service.IsOnline))
        {
            if (!service.IsOnline)
            {
                try
                {
                    await service.ConnectAsync();
#if DEBUG
                    await alertService.ShowAlertAsync("Reconnect", $"{service.ServiceName} reestablished.");
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    await alertService.ShowAlertAsync("Connection Error", $"Service: {service.ServiceName}, Error: {ex.Message}");
#endif
                }
            }
        }
    }
}

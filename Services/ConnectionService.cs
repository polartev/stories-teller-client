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
                service.DisconnectAsync();
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
            if (service.IsOnline == null)
            {
#if DEBUG
                await alertService.ShowAlertAsync("Debug", $"service.IsOnline of {service.ServiceName} is null. Check initialization. (MainViewModel.cs:30)");
#endif
                return;
            }

            bool isOnline = (bool)service.IsOnline;

            if (!isOnline)
            {
                try
                {
                    await service.ConnectAsync();

                    isOnline = service.IsOnline == null ? false : (bool)service.IsOnline;

                    while (!isOnline)
                    {
                        if (isOnline)
                        {
#if DEBUG
                            await alertService.ShowAlertAsync("Connection Info", $"{service.ServiceName} established from ConnectionService.");
#endif
                        }
                        else
                        {
                            await Task.Delay(2000);
                            await service.ConnectAsync();

                            isOnline = service.IsOnline == null ? false : (bool)service.IsOnline;
#if DEBUG
                            await alertService.ShowAlertAsync("Connection Error", $"{service.ServiceName} could not be established from ConnectionService.");
#endif
                        }
                    }
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
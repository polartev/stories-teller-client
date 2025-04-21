using Story_Teller.IServices;

namespace Story_Teller.Services;

public class AlertService : IAlertService
{
    public Task ShowAlertAsync(string title, string message)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Shell.Current.DisplayAlert(title, message, "Ok");
        });
        return Task.CompletedTask;
    }

    public Task<bool> ShowConfirmationAsync(string title, string message)
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            bool result = await Shell.Current.DisplayAlert(title, message, "Yes", "No");
            taskCompletionSource.SetResult(result);
        });
        return taskCompletionSource.Task;
    }
}
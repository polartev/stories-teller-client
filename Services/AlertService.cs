using Story_Teller.IServices;

namespace Story_Teller.Services;

public class AlertService : IAlertService
{
    public async Task ShowAlertAsync(string title, string message)
    {
        for (int i = 0; i < 300; i++)
        {
            try
            {
                if (Shell.Current.CurrentPage != null)
                {
                    if (Shell.Current.CurrentPage.IsLoaded)
                    {
                        break;
                    }
                }
            }
            catch { }

            await Task.Delay(100);
        }

        try
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (Shell.Current.CurrentPage != null)
                {
                    if (Shell.Current.CurrentPage.IsLoaded)
                    {
                        await Shell.Current.DisplayAlert(title, message, "OK");
                    }
                }
            });
        }
        catch { }
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
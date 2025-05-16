using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Http.Headers;

namespace Story_Teller.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private byte[]? image;

    [ObservableProperty]
    private UserViewModel? user;

    [ObservableProperty]
    private StoryViewModel? story;

    private IServices.IAlertService alertService;
    private IServices.IConnectionService connectionService;
    private IServices.IWebSocketService webSocketService;

    public MainViewModel(IServices.IAlertService alertService,
        IServices.IConnectionService connectionService,
        IServices.IWebSocketService webSocketService,
        UserViewModel userViewModel,
        StoryViewModel storyViewModel)
    {
        User = userViewModel;
        Story = storyViewModel;

        this.alertService = alertService;
        this.connectionService = connectionService;
        this.webSocketService = webSocketService;

        this.webSocketService.OnMessageReceived += OnMessageReceived;
        this.connectionService.StartMonitoring(this.webSocketService);
    }

    [RelayCommand]
    private async Task OnStoryButtonTappedAsync()
    {
        try
        {
            if (Image == null)
            {
#if DEBUG
                await alertService.ShowAlertAsync("Debug", "Image is null. Check what happens in MainPage.xaml.cs. (MainViewModel.cs:30)");
#else
                await alertService.ShowAlertAsync("Error", "The image could not be processed. Please try again.");
#endif
                return;
            }

            var content = new MultipartFormDataContent();
            var byteContent = new ByteArrayContent(Image);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(byteContent, "file", $"{User.Name}_img_{DateTime.UtcNow:yyyyMMdd_HHmmss}");

            var url = $"https://api.stories-teller.com/upload?user_id={User.Name}";
            var client = new HttpClient();
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                await alertService.ShowAlertAsync("Success", "Image uploaded successfully.");
            }
            else
            {
#if DEBUG
                await alertService.ShowAlertAsync("Debug", $"Upload failed: {response.ReasonPhrase}");
#else
                await alertService.ShowAlertAsync("Error", "The image could not be processed. Please try again.");
#endif
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            await alertService.ShowAlertAsync("Debug", $"Error: {ex.Message}");
#endif
        }
    }

    private void OnMessageReceived(string message)
    {
        if (string.IsNullOrEmpty(message) || Story == null)
        {
            return;
        }

        string content = "";

        if (message.StartsWith("new_description:"))
        {
            content = message.Split(':')[1];
        }
        else
        {
            return;
        }

        MainThread.BeginInvokeOnMainThread(() =>
        {
            Story.Content = content;
        });
    }
}
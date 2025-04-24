using CommunityToolkit.Mvvm.ComponentModel;
using System.Net.Http.Headers;

namespace Story_Teller.ViewModels;

public partial class MainViewModel : ObservableObject
{
    // DELETE THESE LINES!!!! JUST FOR DEBUGGING!!! IMPLEMENT USER CLASSES AND DATABASE LATER
    private string Username => "artem";

    // DELETE THESE LINES!!!! JUST FOR DEBUGGING!!! IMPLEMENT USER CLASSES AND DATABASE LATER

    [ObservableProperty]
    private byte[]? image;

    [ObservableProperty]
    private StoryViewModel? story;

    private IServices.IAlertService alertService;
    private IServices.IConnectionService connectionService;
    private IServices.IWebSocketService webSocketService;

    public MainViewModel(IServices.IAlertService alertService, 
        IServices.IConnectionService connectionService,
        IServices.IWebSocketService webSocketService)
    {
        Story = new StoryViewModel(new Models.Story());

        this.alertService = alertService;
        this.connectionService = connectionService;
        this.webSocketService = webSocketService;

        webSocketService.OnMessageReceived += OnMessageReceived;
        connectionService.StartMonitoring(webSocketService);
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

    partial void OnImageChanged(byte[]? value)
    {
        _ = OnImageChangedAsync();
    }

    private async Task OnImageChangedAsync()
    {
        if (Image == null) {
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
        content.Add(byteContent, "file", $"{Username}_img_{DateTime.UtcNow:yyyyMMdd_HHmmss}");

        var url = $"https://api.stories-teller.com/upload?user_id={Username}";
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
}
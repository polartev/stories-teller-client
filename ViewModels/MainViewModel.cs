using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Story_Teller.Views;
using System.Net.Http.Headers;

namespace Story_Teller.ViewModels;

public partial class MainViewModel : ObservableObject
{
    // DELETE THESE LINES!!!! JUST FOR DEBUGGING!!! IMPLEMENT USER CLASSES AND DATABASE LATER
    private string Username => "artem";

    // DELETE THESE LINES!!!! JUST FOR DEBUGGING!!! IMPLEMENT USER CLASSES AND DATABASE LATER

    [ObservableProperty]
    private StoryViewModel? story;

    [ObservableProperty]
    private byte[]? image;

    private Services.WebSocketService? webSocketService;

    public MainViewModel()
    {
        Story = new StoryViewModel(new Models.Story());
        webSocketService = new Services.WebSocketService(Username);

        webSocketService.OnMessageReceived += OnMessageReceived;
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

    public async Task InitializeAsync()
    {
        if (webSocketService == null)
        {
#if DEBUG
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.DisplayAlert("Debug", "WebSocketService is null. Check initialization. (MainViewModel.cs:30)", "OK");
            });
#else
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.DisplayAlert("Error", "Core components are missing. Please restart the app.", "OK");
            });
#endif
            return;
        }
        await webSocketService.ConnectAsync();
    }

    partial void OnImageChanged(byte[]? value)
    {
        _ = OnImageChangedAsync();
    }

    private async Task OnImageChangedAsync()
    {
        if (Image == null) {
#if DEBUG
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.DisplayAlert("Exception", $"Image is null. Check what happens in MainPage.xaml.cs. (MainViewModel.cs:30)", "OK");
            });
#else
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.DisplayAlert("Error", "The image could not be processed. Please try again.", "ОК");
            });
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
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.DisplayAlert("Success", "Image uploaded successfully.", "OK");
            });
        }
        else
        {
#if DEBUG
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.DisplayAlert("Error", $"Upload failed: {response.ReasonPhrase}", "OK");
            });
#else
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.DisplayAlert("Error", "The image could not be processed. Please try again.", "ОК");
            });
#endif
        }
    }
}

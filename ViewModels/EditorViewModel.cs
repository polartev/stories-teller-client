using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Story_Teller.ViewModels;

public partial class EditorViewModel : ObservableObject, IDisposable
{
    private bool disposed = false;

    [ObservableProperty]
    private byte[]? image;

    [ObservableProperty]
    private ImageSource? imageSource;

    [ObservableProperty]
    private UserViewModel? user;

    [ObservableProperty]
    private StoryViewModel? story;

    private IServices.IAlertService alertService;
    private IServices.ILanguageService languageService;
    private IServices.IConnectionService connectionService;
    private IServices.IWebSocketService webSocketService;

    public EditorViewModel(IServices.IAlertService alertService,
        IServices.ILanguageService languageService,
        IServices.IConnectionService connectionService,
        IServices.IWebSocketService webSocketService,
        UserViewModel userViewModel)
    {
        User = userViewModel;

        this.alertService = alertService;
        this.languageService = languageService;
        this.connectionService = connectionService;
        this.webSocketService = webSocketService;

        this.webSocketService.OnMessageReceived += OnMessageReceived;

        bool isOnline = webSocketService.IsOnline == null ? false : (bool)webSocketService.IsOnline;
        if (!isOnline)
        {
            this.connectionService.StartMonitoring(this.webSocketService);
        }
    }

    partial void OnImageChanged(byte[]? value)
    {
        if (image != null)
        {
            ImageSource = ImageSource.FromStream(() => new MemoryStream(image));
        }
        else
        {
            ImageSource = null;
        }
    }

    [RelayCommand]
    private Task OnImageButtonTapped()
    {
        if (Image != null)
        {
            Image = null;
        }

        return Task.CompletedTask;
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
            content.Add(new StringContent(languageService.GetLanguage()), "language");

            var url = $"https://api.stories-teller.com/upload?user_id={User.Name}";
            var client = new HttpClient();
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var returnedMessage = await response.Content.ReadFromJsonAsync<Models.Message>();
                if (returnedMessage != null)
                {
                    if (returnedMessage.Type == Models.MessageType.success)
                    {
                        if (returnedMessage.Payload.Action == "file_uploaded")
                        {
                            if (Image != null)
                            {
                                Image = null;
                            }

                            await alertService.ShowAlertAsync("Success", "Image uploaded successfully.");
                        }
                    }
                }
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

    private async void OnMessageReceived(string raw)
    {
        if (string.IsNullOrEmpty(raw) || Story == null)
        {
            return;
        }

        Models.Message? message;

        try
        {
            message = JsonSerializer.Deserialize<Models.Message>(raw);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to deserialize message: {ex.Message}");
            return;
        }

        switch (message?.Type)
        {
            case Models.MessageType.task:
                if (message.Payload.Action == "load_description")
                {
                    var data = message.Payload.Data;
                    if (data.TryGetValue("filename", out var filenameObj))
                    {
                        string? filename = filenameObj?.ToString();
                        if (data.TryGetValue("username", out var usernameObj))
                        {
                            string? username = usernameObj?.ToString();
                            if (User.Name != username)
                            {
                                return;
                            }
                        }
                        var url = $"https://api.stories-teller.com/descriptions/{filename}?user_id={User.Name}";
                        var client = new HttpClient();
                        var response = await client.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseBody = await response.Content.ReadAsStringAsync();
                            var json = JsonDocument.Parse(responseBody);

                            if (json.RootElement.TryGetProperty("description", out var desccriptionObj))
                            {
                                string description = desccriptionObj.GetString()!;
                                Story.Content = description;
                            }
                        }
                        else
                        {
#if DEBUG
                            await alertService.ShowAlertAsync("Debug", $"Error: {response.StatusCode}, {response.ReasonPhrase}");
#else
                            await alertService.ShowAlertAsync("Error", "Please try again.");
#endif
                        }
                    }
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Disposes the CameraViewModel and releases resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the resources used by the CameraViewModel.
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                this.webSocketService.OnMessageReceived -= OnMessageReceived;
            }

            disposed = true;
        }
    }

    /// <summary>
    /// Destructor for CameraViewModel.
    /// </summary>
    ~EditorViewModel()
    {
        Dispose(disposing: false);
    }
}
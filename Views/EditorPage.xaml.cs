using CommunityToolkit.Maui.Views;
using Story_Teller.ViewModels;
using System.Diagnostics;

namespace Story_Teller.Views;

[QueryProperty(nameof(Story), "Story")]
public partial class EditorPage : ContentPage
{
    public StoryViewModel? Story
    {
        get => editorViewModel?.Story;
        set
        {
            if (editorViewModel != null && value != null)
                editorViewModel.Story = value;
        }
    }

    private EditorViewModel? editorViewModel;

    public EditorPage(EditorViewModel editorViewModel)
    {
        InitializeComponent();
        BindingContext = editorViewModel;
        this.editorViewModel = editorViewModel;
    }

    string GetCurrentWidthState(double width, string name)
        => width >= 1200 ? $"LargeWidth{name}"
           : width >= 800 ? $"MediumWidth{name}"
                       : $"SmallWidth{name}";

    string GetCurrentHeightState(double height, string name)
        => height >= 600 ? $"LargeHeight{name}"
           : height >= 500 ? $"MediumHeight{name}"
                      : $"SmallHeight{name}";

    protected override void OnAppearing()
    {
        base.OnAppearing();

        VisualStateManager.GoToState(main, GetCurrentWidthState(Width, "Main"));
        VisualStateManager.GoToState(story, GetCurrentWidthState(Width, "InnerGrid"));
        VisualStateManager.GoToState(main, GetCurrentHeightState(Height, "Main"));
    }

    private async void CameraMenuClicked(object sender, EventArgs e)
    {
        var result = await this.ShowPopupAsync(new Popups.ImageSourcePopup());

        switch (result as string)
        {
            case "Photos":
                await PickFileAsync();
                break;
            case "Camera":
                await TakePhotoAsync();
                break;
        }
    }

    private async Task PickFileAsync()
    {
        if (editorViewModel == null)
        {
#if DEBUG
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Debug", "MainViewModel is missing. Check App initialization. (MainPage.xaml.cs:134)", "OK");
            });
#else
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Error", "Core components are missing. Please restart the app.", "OK");
            });
#endif
            return;
        }

        try
        {
            FileResult? result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Choose file",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                var stream = await result.OpenReadAsync();

                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                editorViewModel.Image = memoryStream.ToArray();
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Exception", $"Choose file failed: {ex.Message}", "OK");
            });
#else
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Error", "Unable to access the file system. Please check permissions and try again.", "OK");
            });
#endif
        }
    }

    private async Task TakePhotoAsync()
    {
        if (editorViewModel == null)
        {
#if DEBUG
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Debug", "MainViewModel is missing. Check App initialization. (MainPage.xaml.cs:169)", "OK");
            });
#else
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Error", "Core components are missing. Please restart the app.", "OK");
            });
#endif
            return;
        }

        try
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                FileResult? photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo != null)
                {
                    var stream = await photo.OpenReadAsync();

                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    editorViewModel.Image = memoryStream.ToArray();
                }
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Exception", $"CaptureImage failed: {ex.Message}", "OK");
            });
#else
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Error", "Unable to access the camera. Please check permissions and try again.", "OK");
            });
#endif
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (BindingContext is IDisposable disposable)
        {
            disposable.Dispose();
        }

        BindingContext = null;
    }
}
using Story_Teller.ViewModels;
using System.Diagnostics;

namespace Story_Teller.Views;

public partial class MainPage : ContentPage
{
    string GetCurrentWidthState(double width, string name)
    => width >= 1200 ? $"LargeWidth{name}"
       : width >= 800 ? $"MediumWidth{name}"
                   : $"SmallWidth{name}";

    string GetCurrentHeightState(double height, string name)
        => height >= 600 ? $"LargeHeight{name}"
           : height >= 500 ? $"MediumHeight{name}"
                      : $"SmallHeight{name}";

    private MainViewModel? mainViewModel;

    public MainPage(MainViewModel mainViewModel)
    {
        InitializeComponent();
        BindingContext = mainViewModel;
        this.mainViewModel = mainViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        VisualStateManager.GoToState(main, GetCurrentWidthState(Width, "Main"));
        VisualStateManager.GoToState(main, GetCurrentHeightState(Height, "Main"));
    }

    private async void CameraMenuButton_Clicked(object sender, EventArgs e)
    {
        string result = await DisplayActionSheet(null, null, null, "Photo", "Camera");

        switch (result as string)
        {
            case "Photo":
                await PickFileAsync();
                break;
            case "Camera":
                await TakePhotoAsync();
                break;
        }
    }

    private async Task PickFileAsync()
    {
        if (mainViewModel == null)
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
                mainViewModel.Image = memoryStream.ToArray();
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
        if (mainViewModel == null)
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
                    mainViewModel.Image = memoryStream.ToArray();
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
}
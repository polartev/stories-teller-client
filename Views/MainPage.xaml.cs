using Story_Teller.ViewModels;

namespace Story_Teller.Views
{
    public partial class MainPage : ContentPage
    {
        private MainViewModel? mainViewModel;

        public MainPage(MainViewModel mainViewModel)
        {
            InitializeComponent();
            BindingContext = mainViewModel;
            this.mainViewModel = mainViewModel;
        }

        private async void Camera_MediaCaptured(object sender, CommunityToolkit.Maui.Views.MediaCapturedEventArgs e)
        {
            if (mainViewModel == null)
            {
#if DEBUG
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Debug", "MainViewModel is missing. Check App initialization. (MainPage.xaml.cs:17)", "OK");
                });
#else
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Error", "Core components are missing. Please restart the app.", "OK");
                });
#endif
                return;
            }

            if (e.Media == null)
            {
#if DEBUG
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Debug", "Media is null. Check camera capture.", "OK");
                });
#else
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Error", "Failed to capture image. Please try again.", "OK");
                });
#endif
                return;
            }

            try
            {
                using var ms = new MemoryStream();
                await e.Media.CopyToAsync(ms);
                mainViewModel.Image = ms.ToArray();
            }
            catch (Exception ex)
            {
#if DEBUG
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Exception", $"Capture failed: {ex.Message}", "OK");
                });
#else
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Error", "The image could not be processed. Please try again.", "ОК");
                });
#endif
            }
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Camera.CaptureImage(CancellationToken.None);
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
}
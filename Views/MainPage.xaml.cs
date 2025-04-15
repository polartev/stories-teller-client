using CommunityToolkit.Maui.Views;
using System.Net.Http.Headers;

namespace Story_Teller.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await App.MainViewModel.InitializeAsync();
        }

        private async void Camera_MediaCaptured(object sender, CommunityToolkit.Maui.Views.MediaCapturedEventArgs e)
        {
            if (App.MainViewModel == null)
            {
                #if DEBUG
                    await DisplayAlert("Debug", "MainViewModel is missing. Check App initialization. (MainPage.xaml.cs:17)", "OK");
                #else
                    await DisplayAlert("Error", "Core components are missing. Please restart the app.", "OK");
                #endif
                return;
            }

            try
            {
                using var ms = new MemoryStream();
                await e.Media.CopyToAsync(ms);
                App.MainViewModel.Image = ms.ToArray();
            }
            catch (Exception ex)
            {
                #if DEBUG
                    await DisplayAlert("Exception", $"Capture failed: {ex.Message}", "OK");
                #else
                    await DisplayAlert("Error", "The image could not be processed. Please try again.", "ОК");
                #endif
            }
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await Camera.CaptureImage(CancellationToken.None);
        }
    }
}
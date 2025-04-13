namespace Story_Teller
{
    public partial class App : Application
    {
        public static ViewModels.MainViewModel? MainViewModel { get; private set; }

        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            MainViewModel = new ViewModels.MainViewModel();

            return new Window(new AppShell());
        }
    }
}
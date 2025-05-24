namespace Story_Teller;

public partial class AppShell : Shell
{
    private ViewModels.FlyoutFooterViewModel flyoutFooterViewModel;

    public AppShell(ViewModels.FlyoutFooterViewModel flyoutFooterViewModel)
    {
        InitializeComponent();

        if (Application.Current != null)
        {
            // Set the default theme to light if Application.Current is not null
            Application.Current.UserAppTheme = AppTheme.Light;
        }

        BindingContext = flyoutFooterViewModel;
        this.flyoutFooterViewModel = flyoutFooterViewModel;

        Routing.RegisterRoute(nameof(Views.EditorPage), typeof(Views.EditorPage));
    }
}
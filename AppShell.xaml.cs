namespace Story_Teller;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        if (Application.Current != null)
        {
            // Set the default theme to light if Application.Current is not null
            Application.Current.UserAppTheme = AppTheme.Light;
        }
    }
}
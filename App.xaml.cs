namespace Story_Teller;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        if (Current == null)
        {
            throw new InvalidOperationException("Current application instance is null.");
        }

        var mauiContext = Current.Handler?.MauiContext;
        if (mauiContext == null)
        {
            throw new InvalidOperationException("MauiContext is null.");
        }

        var appShell = mauiContext.Services.GetService<AppShell>();
        if (appShell == null)
        {
            throw new InvalidOperationException("AppShell service is not registered.");
        }

        return new Window(appShell);
    }
}

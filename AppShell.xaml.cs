namespace Team_Aura_Period_Tracker_;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));
    }
}
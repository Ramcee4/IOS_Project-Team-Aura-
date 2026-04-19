namespace Team_Aura_Period_Tracker_;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(Up), typeof(Up));
        Routing.RegisterRoute(nameof(Step1Page), typeof(Step1Page));
        Routing.RegisterRoute(nameof(Step2Page), typeof(Step2Page));
        Routing.RegisterRoute(nameof(Step3Page), typeof(Step3Page));
        Routing.RegisterRoute(nameof(Step3_1Page), typeof(Step3_1Page));
        Routing.RegisterRoute(nameof(Step4Page), typeof(Step4Page));
        Routing.RegisterRoute(nameof(FivePageStep), typeof(FivePageStep));
        Routing.RegisterRoute(nameof(SixPageStep), typeof(SixPageStep));
    }
}
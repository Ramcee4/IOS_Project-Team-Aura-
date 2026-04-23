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
        Routing.RegisterRoute(nameof(ForgotPasswordPage), typeof(ForgotPasswordPage));
        Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
        Routing.RegisterRoute(nameof(DailyLogPage), typeof(DailyLogPage));
        Routing.RegisterRoute(nameof(InsightPage), typeof(InsightPage));
        Routing.RegisterRoute(nameof(LearnPage), typeof(LearnPage));
        Routing.RegisterRoute(nameof(AddJournalEntryPage), typeof(AddJournalEntryPage));
    }
}
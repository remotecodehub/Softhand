using UraniumUI; 
using CommunityToolkit.Maui;
using Mopups.Hosting;
namespace Softhand;

public static class MauiProgramExtensions
{
    public static MauiAppBuilder ConfigureUI(this MauiAppBuilder builder)
    {
        builder 
            .UseUraniumUI()
            .UseUraniumUIMaterial()
            .ConfigureMopups()
            .ConfigureFonts(fonts =>
            {
                fonts.AddMaterialSymbolsFonts();
                fonts.AddFontAwesomeIconFonts();
            });
        return builder;
    }
    public static MauiAppBuilder AddLogging(this MauiAppBuilder builder)
    {
#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder;
    }
    public static MauiAppBuilder AddHandlers(this MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers((x) =>
        {
			x.AddHandler<CallPage, CallPageHandler>();
        });
        return builder;
    }
    public static MauiAppBuilder AddViewsAndViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddSingletonWithShellRoute<AVConfigPage, AVConfigViewModel>(Routes.AvConfigPage);
        builder.Services.AddSingletonWithShellRoute<AccountConfigPage, AccountConfigViewModel>(Routes.AccountConfigPage);
        builder.Services.AddSingletonWithShellRoute<BuddyPage, BuddyViewModel>(Routes.BuddyPage);
        builder.Services.AddSingletonWithShellRoute<BuddyConfigPage, BuddyConfigViewModel>(Routes.BuddyConfigPage);
        builder.Services.AddTransientWithShellRoute<CallPage, CallViewModel>(Routes.CallPage);
        return builder;
    }
     
    public static MauiAppBuilder AddServices(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton(typeof(INavigationContextService<>), typeof(NavigationContextService<>));
        builder.Services.AddSingleton<ISoftApp, SoftApp>();
        return builder;
    }
    public static MauiApp CreateMauiApp(this MauiAppBuilder builder)
    {
        return builder
        .UseMauiApp<App>()
        .ConfigureUI()
        .AddLogging()
        .AddHandlers()
        .UseMauiCommunityToolkit()
        .AddServices()
        .AddViewsAndViewModels()
        .Build();
    }
}
 
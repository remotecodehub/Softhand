#if __ANDROID__
using Android.Hardware.Camera2;
using Java.Lang;
using Microsoft.Maui.LifecycleEvents;
#endif
using CommunityToolkit.Maui;
using Mopups.Hosting;
using UraniumUI;
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
        builder.ConfigureMauiHandlers(handlerCollection =>
        {
			handlerCollection.AddHandler<CallPage, CallPageHandler>();
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

#pragma warning disable CA1416
    public static MauiApp CreateMauiApp(this MauiAppBuilder builder) => builder
        .UseMauiApp<App>()
        .ConfigureUI()
        .UseMauiCommunityToolkit()
        .AddLogging()
        .AddServices()
        .AddHandlers()
        .AddViewsAndViewModels()
        .Build();
#pragma warning restore CA1416

}
 
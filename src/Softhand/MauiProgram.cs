using UraniumUI;

namespace Softhand;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseUraniumUI()
            .UseUraniumUIMaterial()
            .ConfigureFonts(fonts =>
			{
                fonts.AddFontAwesomeIconFonts();
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif
#if __ANDROID__
		builder.ConfigureMauiHandlers((x) => {
			x.AddHandler(typeof(CallView), typeof(CallPageRenderer));
		});
#endif
#if __IOS__
		builder.ConfigureMauiHandlers((x) => {
			x.AddHandler(typeof(CallView), typeof(CallPageRenderer));
		});
#endif
        return builder.Build();
	}
}


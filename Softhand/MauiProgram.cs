using Microsoft.Extensions.Logging;
using Softhand.Controls;
#if __ANDROID__
using Softhand.Platforms.Android;
#elif __IOS__
using Softhand.Platforms.iOS;
#endif
namespace Softhand;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("fa-brands-400.ttf", "fa-brands");
                fonts.AddFont("fa-regular-400.ttf", "fa-regular");
                fonts.AddFont("fa-solid-900.ttf", "fa-solid");
                fonts.AddFont("fa-v4compatibility.ttf", "fa-v4compatibility");
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


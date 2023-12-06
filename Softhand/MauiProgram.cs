using Microsoft.Extensions.Logging;
using Softhand.Handlers;

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
		CallPageHandler.Handle();
		return builder.Build();
	}
}


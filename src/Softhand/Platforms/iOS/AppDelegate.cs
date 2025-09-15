using Foundation;

#pragma warning disable IDE0130 // O namespace não corresponde à estrutura da pasta
namespace Softhand;
#pragma warning restore IDE0130 // O namespace não corresponde à estrutura da pasta

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}


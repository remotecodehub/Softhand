#if IOS
using ObjCRuntime;
using UIKit;

#pragma warning disable IDE0130 // O namespace não corresponde à estrutura da pasta
namespace Softhand;
#pragma warning restore IDE0130 // O namespace não corresponde à estrutura da pasta

/// <summary>
/// Entrypoint of ios application
/// </summary>
public class Program
{
    /// <summary>
    /// protected ctor to avoid turn Program class static
    /// </summary>
    protected Program()
    {
        
    }
    // This is the main entry point of the application.
    static void Main(string[] args)
	{
		// if you want to use a different Application Delegate class from "AppDelegate"
		// you can specify it here.
		UIApplication.Main(args, null, typeof(AppDelegate));
	}
}

#endif
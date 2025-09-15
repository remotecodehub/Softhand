using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware.Camera2;
using Android.OS;
using Android.Runtime;
using Java.Lang;

#pragma warning disable IDE0130 // O namespace não corresponde à estrutura da pasta
namespace Softhand;
#pragma warning restore IDE0130 // O namespace não corresponde à estrutura da pasta

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{ 
}


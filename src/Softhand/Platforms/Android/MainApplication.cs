using Android.App;
using Android.Hardware.Camera2;
using Android.Runtime;
using Java.Lang;
using Microsoft.Maui.Platform;

#pragma warning disable IDE0130 // O namespace não corresponde à estrutura da pasta
namespace Softhand;
#pragma warning restore IDE0130 // O namespace não corresponde à estrutura da pasta

[Application]
public class MainApplication: MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
    {
        try
        {
            IntPtr? class_ref = JNIEnv.FindClass("org/pjsip/PjCameraInfo2");
            if (class_ref != null && class_ref.HasValue)
            {
                IntPtr? method_id = JNIEnv.GetStaticMethodID(class_ref.Value,
                    "SetCameraManager", "(Landroid/hardware/camera2/CameraManager;)V");

                if (method_id != null && method_id.HasValue)
                {
                    CameraManager manager = this.GetSystemService(Context.CameraService) as CameraManager;
                    JNIEnv.CallStaticVoidMethod(class_ref.Value, method_id.Value, new JValue(manager));
                    Console.WriteLine("SUCCESS setting cameraManager");
                }
            }

            JavaSystem.LoadLibrary("c++_shared");
            JavaSystem.LoadLibrary("crypto");
            JavaSystem.LoadLibrary("ssl");
            JavaSystem.LoadLibrary("openh264");
            JavaSystem.LoadLibrary("bcg729");
            JavaSystem.LoadLibrary("pjsua2");
        }
        catch (System.Exception ex)
        {
            Android.Util.Log.Error("MainApplication", $"EXCEPTION: {ex}");
            if (ex.InnerException != null)
                Android.Util.Log.Error("MainApplication", $"INNER: {ex.InnerException}");
            throw;
        }
    }


    protected override MauiApp CreateMauiApp()
    {
        try
        {
            return MauiProgram.CreateMauiApp();
        }
        catch (System.Exception e)
        {
            Console.Error.WriteLine(e.Message);
            throw new InvalidOperationException(e.Message, e.InnerException);
        }
    }
}
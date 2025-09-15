#if __ANDROID__
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CommunityToolkit.Mvvm.Messaging;
using Java.Interop;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Button = global::Android.Widget.Button;
namespace Softhand.Platforms.Android;

public class CallPageHandler(IPropertyMapper mapper, CommandMapper commandMapper = null) : ViewHandler<CallPage, ViewGroup>(mapper, commandMapper) 
{
    Button acceptCallButton;
    Button hangupCallButton;
    TextView peerTxt;
    TextView statusTxt;
    private static CallInfo LastCallInfo { get; set; } = new CallInfo();
    private CallPage callPage;
    SurfaceView incomingView; 

    [DllImport("android")]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' em vez de 'DllImportAttribute' para gerar código de marshalling P/Invoke no tempo de compilação
    private static extern IntPtr ANativeWindow_fromSurface(IntPtr jni, IntPtr surface);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' em vez de 'DllImportAttribute' para gerar código de marshalling P/Invoke no tempo de compilação

    protected override ViewGroup CreatePlatformView()
    {
        var activity = Context as Activity;
        var root = new FrameLayout(Context);

        var view = activity.LayoutInflater.Inflate(Resource.Layout.activity_call, root, false);

        incomingView = view.FindViewById<SurfaceView>(Resource.Id.incomingVideoView);

        incomingView.Holder.AddCallback(new SurfaceCallback(this));

        peerTxt = view.FindViewById<TextView>(Resource.Id.peerTxt);
        statusTxt = view.FindViewById<TextView>(Resource.Id.statusTxt);

        acceptCallButton = view.FindViewById<Button>(Resource.Id.acceptCallButton);
        hangupCallButton = view.FindViewById<Button>(Resource.Id.hangupCallButton);

        root.AddView(view);

        return root;
    }

    protected override void ConnectHandler(ViewGroup platformView)
    {
        base.ConnectHandler(platformView);

        callPage = VirtualView;

        SetupEventHandlers();

        WeakReferenceMessenger.Default.Register<UpdateCallStateMessage>(
            this, (obj, info) =>
            {
                if (callPage == null) return;

                LastCallInfo = info.Value;
                if (LastCallInfo.state == pjsip_inv_state.PJSIP_INV_STATE_DISCONNECTED)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        callPage.Navigation.PopAsync();
                    });
                }
                else
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        UpdateCallState(LastCallInfo);
                    });
                }
            });

        WeakReferenceMessenger.Default.Register<UpdateMediaCallStateMessage>(
            this, (obj, info) =>
            {
                LastCallInfo = info.Value;
                if (SoftApp.CurrentCall?.VudeoWindow != null)
                {
                    incomingView.Visibility = ViewStates.Visible;
                }
            });

        if (SoftApp.CurrentCall != null)
        {
            try
            {
                LastCallInfo = SoftApp.CurrentCall.getInfo();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"ERROR: ", ex.Message);
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                UpdateCallState(LastCallInfo);
            });
        }
        else
        {
            incomingView.Visibility = ViewStates.Gone;
        }
    }

    protected override void DisconnectHandler(ViewGroup platformView)
    {
        UpdateVideoWindow(false);

        WeakReferenceMessenger.Default.Unregister<UpdateCallStateMessage>(this);
        WeakReferenceMessenger.Default.Unregister<UpdateMediaCallStateMessage>(this);

        base.DisconnectHandler(platformView);
    }

    void SetupEventHandlers()
    {
        acceptCallButton.Click += AcceptCallButtonTapped;
        hangupCallButton.Click += HangupCallButtonTapped;
    }

    void AcceptCallButtonTapped(object sender, EventArgs e)
    {
        CallOpParam prm = new()
        {
            statusCode = pjsip_status_code.PJSIP_SC_OK
        };
        try
        {
            SoftApp.CurrentCall?.answer(prm);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(@"ERROR: ", ex.Message);
        }

        acceptCallButton.Visibility = ViewStates.Gone;
    }

    static void HangupCallButtonTapped(object sender, EventArgs e)
    {
        if (SoftApp.CurrentCall != null)
        {
            CallOpParam prm = new()
            {
                statusCode = pjsip_status_code.PJSIP_SC_DECLINE
            };
            try
            {
                SoftApp.CurrentCall.hangup(prm);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"ERROR: ", ex.Message);
            }
        }
    }
    private void UpdateVideoWindow(bool show)
    {
        if (SoftApp.CurrentCall != null &&
            SoftApp.CurrentCall.VudeoWindow != null &&
            SoftApp.CurrentCall.VideoPreview != null &&
            incomingView?.Holder?.Surface != null &&
            incomingView.Holder.Surface.IsValid)
        {
            long windHandle = 0;
            VideoWindowHandle vidWH = new();

            if (show)
            {
                try
                {
                    IntPtr winPtr = ANativeWindow_fromSurface(
                        JNIEnv.Handle, incomingView.Holder.Surface.Handle);
                    windHandle = winPtr.ToInt64();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(@"ERROR getting ANativeWindow: " + ex.Message);
                    return;
                }
            }

            vidWH.handle.setWindow(windHandle);

            try
            {
                SoftApp.CurrentCall.VudeoWindow.setWindow(vidWH);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"ERROR setting video window: " + ex.Message);
            }
        }
    }
    private void UpdateCallState(CallInfo ci)
    {
        string call_state = "";

        if (ci == null)
        {
            acceptCallButton.Visibility = ViewStates.Gone;
            hangupCallButton.Text = "OK";
            statusTxt.Text = "Call disconnected";
            return;
        }

        if (ci.role == pjsip_role_e.PJSIP_ROLE_UAC)
        {
            acceptCallButton.Visibility = ViewStates.Gone;
        }

        if (ci.state < pjsip_inv_state.PJSIP_INV_STATE_CONFIRMED)
        {
            if (ci.role == pjsip_role_e.PJSIP_ROLE_UAS)
            {
                call_state = "Incoming call..";
            }
            else
            {
                hangupCallButton.Text = "Cancel";
                call_state = ci.stateText;
            }
        }
        else if (ci.state >= pjsip_inv_state.PJSIP_INV_STATE_CONFIRMED)
        {
            acceptCallButton.Visibility = ViewStates.Gone;
            call_state = ci.stateText;
            if (ci.state == pjsip_inv_state.PJSIP_INV_STATE_CONFIRMED)
            {
                hangupCallButton.Text = "Hangup";
            }
            else if (ci.state == pjsip_inv_state.PJSIP_INV_STATE_DISCONNECTED)
            {
                hangupCallButton.Text = "OK";
                call_state = "Call disconnected: " + ci.lastReason;
            }
            if (ci.state == pjsip_inv_state.PJSIP_INV_STATE_CONFIRMED)
            {
                UpdateVideoWindow(true);
            }
        }

        peerTxt.Text = ci.remoteUri;
        statusTxt.Text = call_state;
    }

    #region ISurfaceHolderCallback
    // Delegates chamados pela SurfaceCallback
    internal void OnSurfaceChanged(ISurfaceHolder holder, Format format, int width, int height)
    {
        try { UpdateVideoWindow(true); }
        catch (Exception e) { System.Diagnostics.Debug.WriteLine("Error on SurfaceChanged: " + e.Message); }
    }

    internal void OnSurfaceCreated(ISurfaceHolder holder)
    {
        try { UpdateVideoWindow(true); }
        catch (Exception e) { System.Diagnostics.Debug.WriteLine("Error on SurfaceCreated: " + e.Message); }
    }

    internal void OnSurfaceDestroyed(ISurfaceHolder holder)
    {
        try { UpdateVideoWindow(false); }
        catch (Exception e) { System.Diagnostics.Debug.WriteLine("Error on SurfaceDestroyed: " + e.Message); }
    }
    #endregion
    
}
#endif
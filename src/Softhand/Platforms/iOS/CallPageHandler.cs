#if __IOS__
using CommunityToolkit.Mvvm.Messaging;
using CoreGraphics;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Handlers;
using Softhand.Application.Controls;
using Softhand.Infrastructure.Messages;
using System;
using UIKit;

namespace Softhand.Platforms.iOS;

public class CallPageHandler : ViewHandler<CallPage, UIView>
{
    UIView incomingVideoView;
    UIButton acceptCallButton;
    UIButton hangupCallButton;
    UILabel peerLabel;
    UILabel callStatusLabel;
    private static CallInfo lastCallInfo { get; set; } = new CallInfo();
    private CallPage callPage;

    public CallPageHandler(IPropertyMapper mapper, CommandMapper commandMapper = null) : base(mapper, commandMapper)
    {
       
    }

    protected override UIView CreatePlatformView()
    {
        var rootView = new UIView();

        var controlWidth = 150;
        var controlHeight = 50;
        var centerButtonX = rootView.Bounds.GetMidX() - 35f;
        var topLeftX = rootView.Bounds.X + 25;
        var topRightX = rootView.Bounds.Right - controlWidth - 25;
        var bottomButtonY = rootView.Bounds.Bottom - 150;
        var bottomLabelY = rootView.Bounds.Top + 15;

        incomingVideoView = new UIView()
        {
            Frame = new CGRect(0f, 0f, rootView.Bounds.Width,
                               rootView.Bounds.Height)
        };

        acceptCallButton = new UIButton()
        {
            Frame = new CGRect(topLeftX, bottomButtonY, controlWidth,
                               controlHeight)
        };
        acceptCallButton.SetTitle("Accept", UIControlState.Normal);
        acceptCallButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
        acceptCallButton.BackgroundColor = UIColor.White;

        hangupCallButton = new UIButton()
        {
            Frame = new CGRect(topRightX, bottomButtonY, controlWidth,
                               controlHeight)
        };
        hangupCallButton.SetTitle("Hangup", UIControlState.Normal);
        hangupCallButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
        hangupCallButton.BackgroundColor = UIColor.White;

        peerLabel = new UILabel
        {
            TextAlignment = UITextAlignment.Center,
            Frame = new CGRect(rootView.Bounds.X,
                               bottomLabelY,
                               rootView.Bounds.Right,
                               controlHeight)
        };

        callStatusLabel = new UILabel
        {
            TextAlignment = UITextAlignment.Center,
            Frame = new CGRect(rootView.Bounds.X,
                               bottomLabelY + controlHeight,
                               rootView.Bounds.Right,
                               controlHeight)
        };

        rootView.Add(incomingVideoView);
        rootView.Add(acceptCallButton);
        rootView.Add(hangupCallButton);
        rootView.Add(peerLabel);
        rootView.Add(callStatusLabel);

        SetupEventHandlers();

        return rootView;
    }

    protected override void ConnectHandler(UIView platformView)
    {
        base.ConnectHandler(platformView);

        callPage = VirtualView;

        WeakReferenceMessenger.Default.Register<UpdateCallStateMessage>(
            this,  (obj, message) =>
            {
                SoftApp.LastCallInfo = message.Value;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateCallState(lastCallInfo);
                });

                if (lastCallInfo.state ==
                    pjsip_inv_state.PJSIP_INV_STATE_DISCONNECTED)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        callPage?.Navigation.PopAsync();
                    });
                }
            });

        WeakReferenceMessenger.Default.Register<UpdateMediaCallStateMessage>(
            this, (obj, info) =>
            {
                lastCallInfo = info.Value;

                if (SoftApp.CurrentCall?.VudeoWindow != null)
                {
                    incomingVideoView.Hidden = false;
                }
            });

        if (SoftApp.CurrentCall != null)
        {
            try
            {
                lastCallInfo = SoftApp.CurrentCall.getInfo();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"ERROR: ", ex.Message);
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                UpdateCallState(lastCallInfo);
            });
        }
        else
        {
            incomingVideoView.Hidden = true;
        }
    }

    protected override void DisconnectHandler(UIView platformView)
    {
        UpdateVideoWindow(false);

        WeakReferenceMessenger.Default.Unregister<UpdateCallStateMessage>(this);
        WeakReferenceMessenger.Default.Unregister<UpdateMediaCallStateMessage>(this);

        base.DisconnectHandler(platformView);
    }

    void SetupEventHandlers()
    {
        acceptCallButton.TouchUpInside += (object sender, EventArgs e) =>
        {
            AcceptCall();
        };

        hangupCallButton.TouchUpInside += (object sender, EventArgs e) =>
        {
            HangupCall();
        };
    }

    void AcceptCall()
    {
        CallOpParam prm = new CallOpParam();
        prm.statusCode = pjsip_status_code.PJSIP_SC_OK;
        try
        {
            SoftApp.CurrentCall?.answer(prm);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(@"ERROR: ", ex.Message);
        }

        acceptCallButton.Hidden = true;
    }

    private static void HangupCall()
    {
        if (SoftApp.CurrentCall != null)
        {
            CallOpParam prm = new CallOpParam();
            prm.statusCode = pjsip_status_code.PJSIP_SC_DECLINE;
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

    private void UpdateCallState(CallInfo ci)
    {
        string call_state = "";

        if (ci == null)
        {
            acceptCallButton.Hidden = true;
            hangupCallButton.SetTitle("OK", UIControlState.Normal);
            callStatusLabel.Text = "Call disconnected";
            return;
        }

        if (ci.role == pjsip_role_e.PJSIP_ROLE_UAC)
        {
            acceptCallButton.Hidden = true;
        }

        if (ci.state < pjsip_inv_state.PJSIP_INV_STATE_CONFIRMED)
        {
            if (ci.role == pjsip_role_e.PJSIP_ROLE_UAS)
            {
                call_state = "Incoming call..";
            }
            else
            {
                hangupCallButton.SetTitle("Cancel", UIControlState.Normal);
                call_state = ci.stateText;
            }
        }
        else if (ci.state >= pjsip_inv_state.PJSIP_INV_STATE_CONFIRMED)
        {
            acceptCallButton.Hidden = true;
            call_state = ci.stateText;
            if (ci.state == pjsip_inv_state.PJSIP_INV_STATE_CONFIRMED)
            {
                hangupCallButton.SetTitle("Hangup", UIControlState.Normal);
            }
            else if (ci.state == pjsip_inv_state.PJSIP_INV_STATE_DISCONNECTED)
            {
                hangupCallButton.SetTitle("OK", UIControlState.Normal);
                call_state = "Call disconnected: " + ci.lastReason;
            }
            if (ci.state == pjsip_inv_state.PJSIP_INV_STATE_CONFIRMED)
            {
                UpdateVideoWindow(true);
            }
        }

        peerLabel.Text = ci.remoteUri;
        callStatusLabel.Text = call_state;
    }

    private void UpdateVideoWindow(bool show)
    {
        if (SoftApp.CurrentCall != null &&
            SoftApp.CurrentCall.VudeoWindow != null &&
            SoftApp.CurrentCall.VideoPreview != null &&
            show)
        {
            try
            {
                VideoWindowInfo winInfo = SoftApp.CurrentCall.VudeoWindow.getInfo();

                if (winInfo?.winHandle?.handle == null ||
                    winInfo.winHandle.handle.window == IntPtr.Zero)
                {
                    System.Diagnostics.Debug.WriteLine("UpdateVideoWindow skipped: invalid handle.");
                    return;
                }

                IntPtr winPtr = winInfo.winHandle.handle.window;

                if (ObjCRuntime.Runtime.GetNSObject(winPtr) is not UIView inView)
                {
                    System.Diagnostics.Debug.WriteLine("UpdateVideoWindow skipped: UIView not resolved.");
                    return;
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    try
                    { 
                        if (inView.Superview != incomingVideoView)
                        {
                            incomingVideoView.AddSubview(inView);
                        }

                        inView.ContentMode = UIViewContentMode.ScaleAspectFit;
                        inView.Frame = incomingVideoView.Bounds;
                        inView.Center = incomingVideoView.Center;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("ERROR updating video view: " + ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR in UpdateVideoWindow: " + ex.Message);
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("UpdateVideoWindow skipped: CurrentCall or video handles are null.");
        }
    }

}
#endif
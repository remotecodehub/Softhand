#if __IOS__
using CoreGraphics;
using pjsua2xamarin.pjsua2;
using Softhand.Models;
using Softhand.Views;
using UIKit;

namespace Softhand.Platforms.iOS;


public partial class CallPageHandler
{
    static UIView incomingVideoView;
    static UIButton acceptCallButton;
    static UIButton hangupCallButton;
    static UILabel peerLabel;
    static UILabel callStatusLabel;
    private static CallInfo lastCallInfo;
    private CallPage callPage;
    public CallPageHandler()
    {
        MessagingCenter.Subscribe<BuddyPage, CallInfo>
                    (this, "UpdateCallState", (obj, info) => {
                        lastCallInfo = info as CallInfo;
                        Device.BeginInvokeOnMainThread(() => {
                            updateCallState(lastCallInfo);
                        });

                        if (lastCallInfo.state ==
                            pjsip_inv_state.PJSIP_INV_STATE_DISCONNECTED)
                        {
                            Device.BeginInvokeOnMainThread(() => {
                                //pop call page
                            });
                        }
                    });
        MessagingCenter.Subscribe<BuddyPage, CallInfo>
        (this, "UpdateMediaCallState", (obj, info) => {
            lastCallInfo = info as CallInfo;

            if (SoftApp.currentCall.vidWin != null)
            {
                incomingVideoView.Hidden = false;
            }
        });
    }

    public static void PageHandle()
    {
        Microsoft.Maui.Handlers.ContentViewHandler.ViewCommandMapper["SoftCustomization"] = (handler, view, sender) =>
        {
            var page = (UIView)handler.PlatformView;
            page.BeginInvokeOnMainThread(() => {
                var controlWidth = 150;
                var controlHeight = 50;
                var centerButtonX = (handler.VirtualView.Width/2) - 35f;
                var topLeftX = handler.VirtualView.Width + 25;
                var topRightX = handler.VirtualView.Margin.Right - controlWidth - 25;
                var bottomButtonY = handler.VirtualView.Margin.Bottom - 150;
                var bottomLabelY = handler.VirtualView.Margin.Top + 15;

                CallPageHandler.incomingVideoView = new UIView()
                {
                    Frame = new CGRect(0f, 0f, handler.VirtualView.Width,
                                       handler.VirtualView.Height)
                };

                acceptCallButton = new UIButton()
                {
                    Frame = new CGRect(topLeftX, bottomButtonY, controlWidth,
                                       controlHeight)
                };
                acceptCallButton.SetTitle("Accept", UIControlState.Normal);
                acceptCallButton.SetTitleColor(color: UIColor.Black,
                                               UIControlState.Normal);
                acceptCallButton.BackgroundColor = UIColor.White;

                hangupCallButton = new UIButton()
                {
                    Frame = new CGRect(topRightX, bottomButtonY, controlWidth,
                                       controlHeight)
                };
                hangupCallButton.SetTitle("Hangup", UIControlState.Normal);
                hangupCallButton.SetTitleColor(color: UIColor.Black,
                                               UIControlState.Normal);
                hangupCallButton.BackgroundColor = UIColor.White;

                peerLabel = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    Frame = new CGRect(handler.VirtualView.Width,
                                                           bottomLabelY,
                                                           handler.VirtualView.Margin.Right,
                                                           controlHeight)
                };
                callStatusLabel = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    Frame = new CGRect(handler.VirtualView.Width,
                                                      bottomLabelY + controlHeight,
                                                      handler.VirtualView.Margin.Right,
                                                      controlHeight)
                };

                page.Add(incomingVideoView);
                page.Add(acceptCallButton);
                page.Add(hangupCallButton);
                page.Add(peerLabel);
                page.Add(callStatusLabel);

                acceptCallButton.TouchUpInside += (object sender, EventArgs e) => {
                    CallPageHandler.AcceptCall();
                };

                hangupCallButton.TouchUpInside += (object sender, EventArgs e) => {
                    CallPageHandler.HangupCall();
                };
            }); 
        };
    }


    ~CallPageHandler()
    {
        updateVideoWindow(false);
    }

   

    static void AcceptCall()
    {
        CallOpParam prm = new CallOpParam();
        prm.statusCode = pjsip_status_code.PJSIP_SC_OK;
        try
        {
            SoftApp.currentCall.answer(prm);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(@"ERROR: ", ex.Message);
        }

        acceptCallButton.Hidden = true;
    }

    static void HangupCall()
    {
        if (SoftApp.currentCall != null)
        {
            CallOpParam prm = new CallOpParam();
            prm.statusCode = pjsip_status_code.PJSIP_SC_DECLINE;
            try
            {
                SoftApp.currentCall.hangup(prm);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"ERROR: ", ex.Message);
            }
        }
    }

    private void updateCallState(CallInfo ci)
    {
        String call_state = "";

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

        if (ci.state <
            pjsip_inv_state.PJSIP_INV_STATE_CONFIRMED)
        {
            if (ci.role == pjsip_role_e.PJSIP_ROLE_UAS)
            {
                call_state = "Incoming call..";
                /* Default button texts are already 'Accept' & 'Reject' */
            }
            else
            {
                hangupCallButton.SetTitle("Cancel", UIControlState.Normal);
                call_state = ci.stateText;
            }
        }
        else if (ci.state >=
                   pjsip_inv_state.PJSIP_INV_STATE_CONFIRMED)
        {
            acceptCallButton.Hidden = true;
            call_state = ci.stateText;
            if (ci.state == pjsip_inv_state.PJSIP_INV_STATE_CONFIRMED)
            {
                hangupCallButton.SetTitle("Hangup", UIControlState.Normal);
            }
            else if (ci.state ==
                       pjsip_inv_state.PJSIP_INV_STATE_DISCONNECTED)
            {
                hangupCallButton.SetTitle("OK", UIControlState.Normal);
                call_state = "Call disconnected: " + ci.lastReason;
            }
            if (ci.state == pjsip_inv_state.PJSIP_INV_STATE_CONFIRMED)
            {
                updateVideoWindow(true);
            }
        }

        peerLabel.Text = ci.remoteUri;
        callStatusLabel.Text = call_state;
    }

    private void updateVideoWindow(bool show)
    {
        if (SoftApp.currentCall != null &&
            SoftApp.currentCall.vidWin != null &&
            SoftApp.currentCall.vidPrev != null)
        {
            if (show)
            {
                VideoWindowInfo winInfo =
                                    SoftApp.currentCall.vidWin.getInfo();
                IntPtr winPtr = winInfo.winHandle.handle.window;
                UIView inView =
                           (UIView)ObjCRuntime.Runtime.GetNSObject(winPtr);
                try
                {
                    incomingVideoView.BeginInvokeOnMainThread(() => {
                        incomingVideoView.AddSubview(inView);
                        inView.ContentMode = UIViewContentMode.ScaleAspectFit;
                        inView.Center = incomingVideoView.Center;
                        inView.Frame = incomingVideoView.Bounds;
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(@"ERROR: ",
                                                       ex.Message);
                }
            }
        }
    }
}
#endif
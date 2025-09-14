using pjsua2maui.pjsua2;

namespace Softhand.Domain.Models
{
    public class SoftCall(SoftAccount acc, int call_id) : Call(acc, call_id)
    {
        public VideoWindow VudeoWindow { get; set; } = null;
        public VideoPreview VideoPreview { get; set; } = null;

        public override void onCallState(OnCallStateParam prm)
        {
            try
            {
                CallInfo ci = getInfo();
                if (ci.state ==
                    pjsip_inv_state.PJSIP_INV_STATE_DISCONNECTED)
                {
                    SoftApp.Endpoint.utilLogWrite(3, "SoftCall", dump(true, ""));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error : " + ex.Message);
            }

            // Should not delete this call instance (self) in this context,
            // so the Monitor should manage this call instance deletion
            // out of this callback context.
            SoftApp.Monitor.NotifyCallState(this);
        }

        public override void onCallMediaState(OnCallMediaStateParam prm)
        {
            CallInfo ci;
            try
            {
                ci = getInfo();
            }
            catch (Exception)
            {
                return;
            }

            CallMediaInfoVector cmiv = ci.media;

            for (int i = 0; i < cmiv.Count; i++)
            {
                CallMediaInfo cmi = cmiv[i];
                if (cmi.type == pjmedia_type.PJMEDIA_TYPE_AUDIO &&
                    (cmi.status ==
                            pjsua_call_media_status.PJSUA_CALL_MEDIA_ACTIVE ||
                     cmi.status ==
                            pjsua_call_media_status.PJSUA_CALL_MEDIA_REMOTE_HOLD))
                {
                    // connect ports
                    try
                    {
                        AudDevManager audMgr = SoftApp.Endpoint.audDevManager();
                        AudioMedia am = getAudioMedia(i);
                        audMgr.getCaptureDevMedia().startTransmit(am);
                        am.startTransmit(audMgr.getPlaybackDevMedia());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed connecting media ports" +
                                          e.Message);
                    }
                }
                else if (cmi.type == pjmedia_type.PJMEDIA_TYPE_VIDEO &&
                           cmi.status == pjsua_call_media_status.PJSUA_CALL_MEDIA_ACTIVE &&
                           cmi.videoIncomingWindowId != pjsua2.INVALID_ID)
                {
                    VudeoWindow = new VideoWindow(cmi.videoIncomingWindowId);
                    VideoPreview = new VideoPreview(cmi.videoCapDev);
                }
            }

            SoftApp.Monitor.NotifyCallMediaState(this);
        }
    }
}
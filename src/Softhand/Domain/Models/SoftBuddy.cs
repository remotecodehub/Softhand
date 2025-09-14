using pjsua2maui.pjsua2;

namespace Softhand.Domain.Models;

public class SoftBuddy(BuddyConfig config) : Buddy
{
    public BuddyConfig Configuration { get; set; } = config;

    public string GetStatusText()
    {
        BuddyInfo bi;

        try
        {
            bi = getInfo();
        }
        catch (Exception)
        {
            return "?";
        }

        string status = "";
        if (bi.subState == pjsip_evsub_state.PJSIP_EVSUB_STATE_ACTIVE)
        {
            if (bi.presStatus.status ==
                pjsua_buddy_status.PJSUA_BUDDY_STATUS_ONLINE)
            {
                status = bi.presStatus.statusText;
                if (status == null || status.Length == 0)
                {
                    status = "Online";
                }
            }
            else if (bi.presStatus.status ==
                       pjsua_buddy_status.PJSUA_BUDDY_STATUS_OFFLINE)
            {
                status = "Offline";
            }
            else
            {
                status = "Unknown";
            }
        }
        return status;
    }
    public override void onBuddyEvSubState(OnBuddyEvSubStateParam prm)
    {
        SoftApp.Monitor.NotifyOnBuddyEvSubState(this, prm);
    }
    override public void onBuddyState()
    {
        SoftApp.Monitor.NotifyBuddyState(this);
    }
}
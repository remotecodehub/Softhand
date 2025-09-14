namespace Softhand.Domain.Models;

public class SoftAccount(AccountConfig config) : Account
{
    public List<SoftBuddy> BuddyList { get; set; } = [];
    public AccountConfig Configuration { get; set; } = config;

    ~SoftAccount()
    {
        Console.WriteLine("*** Account is being deleted");
    }

    public SoftBuddy AddBuddy(BuddyConfig bud_cfg)
    {
        /* Create Buddy */
        SoftBuddy bud = new(bud_cfg);
        try
        {
            bud.create(this, bud_cfg);
        }
        catch (Exception)
        {
            bud.Dispose();
        }

        if (bud != null)
        {
            BuddyList.Add(bud);
            if (bud_cfg.subscribe)
                try
                {
                    bud.subscribePresence(true);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                }
        }

        return bud;
    }

    public void DelBuddy(SoftBuddy buddy)
    {
        BuddyList.Remove(buddy);
        buddy.Dispose();
    }

    override public void onRegState(OnRegStateParam prm)
    {
        AccountInfo ai = getInfo();
        Console.WriteLine("***" + (ai.regIsActive ? "" : "Un") +
                          "Register: code=" + prm.code);

        SoftApp.Monitor.notifyRegState((int)prm.code, prm.reason, prm.expiration);
    }

    override public void onIncomingCall(OnIncomingCallParam prm)
    {
        Console.WriteLine("======== Incoming call ======== ");
        SoftCall call = new SoftCall(this, prm.callId);
        SoftApp.Monitor.notifyIncomingCall(call);
    }

    override public void onInstantMessage(OnInstantMessageParam prm)
    {
        Console.WriteLine("======== Incoming pager ======== ");
        Console.WriteLine("From     : " + prm.fromUri);
        Console.WriteLine("To       : " + prm.toUri);
        Console.WriteLine("Contact  : " + prm.contactUri);
        Console.WriteLine("Mimetype : " + prm.contentType);
        Console.WriteLine("Body     : " + prm.msgBody);
    }
}
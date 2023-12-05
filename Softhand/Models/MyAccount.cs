using pjsua2xamarin.pjsua2;

namespace Softhand.Models;

public class MyAccount : Account
{
    public List<MyBuddy> buddyList = new List<MyBuddy>();
    public AccountConfig cfg;

    ~MyAccount()
    {
        Console.WriteLine("*** Account is being deleted");
    }

    public MyAccount(AccountConfig config)
    {
        cfg = config;
    }

    public MyBuddy addBuddy(BuddyConfig bud_cfg)
    {
        /* Create Buddy */
        MyBuddy bud = new MyBuddy(bud_cfg);
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
            buddyList.Add(bud);
            if (bud_cfg.subscribe)
                try
                {
                    bud.subscribePresence(true);
                }
                catch (Exception) { }
        }

        return bud;
    }

    public void delBuddy(MyBuddy buddy)
    {
        buddyList.Remove(buddy);
        buddy.Dispose();
    }

    override public void onRegState(OnRegStateParam prm)
    {
        AccountInfo ai = getInfo();
        Console.WriteLine("***" + (ai.regIsActive ? "" : "Un") +
                          "Register: code=" + prm.code);

        MyApp.observer.notifyRegState((int)prm.code, prm.reason, prm.expiration);
    }

    override public void onIncomingCall(OnIncomingCallParam prm)
    {
        Console.WriteLine("======== Incoming call ======== ");
        MyCall call = new MyCall(this, prm.callId);
        MyApp.observer.notifyIncomingCall(call);
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
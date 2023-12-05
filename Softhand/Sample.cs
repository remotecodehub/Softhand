using pjsua2xamarin.pjsua2;

namespace Softhand;
public class MyAccount : Account
{
    ~MyAccount()
    {
        Console.WriteLine("*** Account is being deleted");
    }

    override public void onRegState(OnRegStateParam prm)
    {
        AccountInfo ai = getInfo();
        Console.WriteLine("***" + (ai.regIsActive ? "" : "Un") +
                      "Register: code=" + prm.code);
    }
}

public class MyLogWriter : LogWriter
{
    override public void write(LogEntry entry)
    {
        Console.WriteLine(entry.msg);
    }
}

public class Sample
{
    public static Endpoint ep = new Endpoint();
    public static MyLogWriter writer = new MyLogWriter();

    public Sample()
    {
    }

    public static void RunSample()
    {
        try
        {
            ep.libCreate();

            // Init library
            EpConfig epConfig = new EpConfig();
            epConfig.logConfig.writer = writer;
            ep.libInit(epConfig);

            // Create transport
            TransportConfig tcfg = new TransportConfig();
            tcfg.port = 5080;
            ep.transportCreate(pjsip_transport_type_e.PJSIP_TRANSPORT_UDP,
                       tcfg);
            ep.transportCreate(pjsip_transport_type_e.PJSIP_TRANSPORT_TCP,
                       tcfg);

            // Start library
            ep.libStart();
            Console.WriteLine("*** PJSUA2 STARTED ***");

            // Add account
            AccountConfig accCfg = new AccountConfig();
            accCfg.idUri = "sip:test1@pjsip.org";
            accCfg.regConfig.registrarUri = "sip:sip.pjsip.org";
            accCfg.sipConfig.authCreds.Add(
                new AuthCredInfo("digest", "*", "test1", 0, "test1"));
            MyAccount acc = new MyAccount();
            acc.create(accCfg);

            Console.WriteLine("*** DESTROYING PJSUA2 ***");
            // Explicitly delete account when unused
            acc.Dispose();
            ep.libDestroy();
        }
        catch (Exception err)
        {
            Console.WriteLine("Exception: " + err.Message);
        }
    }
}


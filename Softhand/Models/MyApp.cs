using pjsua2xamarin.pjsua2;

namespace Softhand.Models;


 






public class MyApp
{
    public static Endpoint ep = new Endpoint();
    public static MyCall currentCall = null;
    public static MyAccount account = null;
    public static MyAccountConfig myAccCfg;
    public static MyAppObserver observer;

    private static MyLogWriter logWriter = new MyLogWriter();
    private EpConfig epConfig = new EpConfig();
    private TransportConfig sipTpConfig = new TransportConfig();
    private String appDir;

    private const String configName = "pjsua2.json";
    private const int SIP_PORT = 6000;
    private const int LOG_LEVEL = 5;

    public MyApp()
    {

    }

    public void init(MyAppObserver obs, String app_dir)
    {
        observer = obs;
        appDir = app_dir;

        /* Create endpoint */
        try
        {
            ep.libCreate();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error init : " + e.Message);
            return;
        }

        myAccCfg = new MyAccountConfig();
        /* Load config */
        String configPath = appDir + "/" + configName;
        if (File.Exists(configPath))
        {
            loadConfig(configPath);
        }
        else
        {
            /* Set 'default' values */
            sipTpConfig.port = SIP_PORT;
        }

        /* Override log level setting */
        epConfig.logConfig.level = LOG_LEVEL;
        epConfig.logConfig.consoleLevel = LOG_LEVEL;

        /* Set log config. */
        LogConfig log_cfg = epConfig.logConfig;
        logWriter = new MyLogWriter();
        log_cfg.writer = logWriter;
        log_cfg.decor += (uint)pj_log_decoration.PJ_LOG_HAS_NEWLINE;

        UaConfig ua_cfg = epConfig.uaConfig;
        ua_cfg.userAgent = "Pjsua2 Xamarin " + ep.libVersion().full;

        /* Init endpoint */
        try
        {
            ep.libInit(epConfig);
        }
        catch (Exception)
        {
            return;
        }

        /* Create transports. */
        try
        {
            ep.transportCreate(pjsip_transport_type_e.PJSIP_TRANSPORT_UDP,
                               sipTpConfig);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        try
        {
            ep.transportCreate(pjsip_transport_type_e.PJSIP_TRANSPORT_TCP,
                               sipTpConfig);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        try
        {
            sipTpConfig.port = SIP_PORT + 1;
            ep.transportCreate(pjsip_transport_type_e.PJSIP_TRANSPORT_TLS,
                               sipTpConfig);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        /* Set SIP port back to default for JSON saved config */
        sipTpConfig.port = SIP_PORT;
        AccountConfig accountConfig = myAccCfg.accCfg;
        if (accountConfig.idUri == "")
        {
            accountConfig.idUri = "sip:localhost";
        }
        accountConfig.natConfig.iceEnabled = true;
        accountConfig.videoConfig.autoTransmitOutgoing = true;
        accountConfig.videoConfig.autoShowIncoming = true;
        accountConfig.mediaConfig.srtpUse = pjmedia_srtp_use.PJMEDIA_SRTP_OPTIONAL;
        accountConfig.mediaConfig.srtpSecureSignaling = 0;

        account = new MyAccount(accountConfig);
        try
        {
            account.create(accountConfig);

            /* Add Buddies */
            foreach (BuddyConfig budCfg in myAccCfg.buddyCfgs)
            {
                account.addBuddy(budCfg);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            account = null;
        }

        /* Start. */
        try
        {
            ep.libStart();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public void deinit()
    {
        String configPath = appDir + "/" + configName;
        saveConfig(configPath);

        /* Shutdown pjsua. Note that Endpoint destructor will also invoke
         * libDestroy(), so this will be a test of double libDestroy().
         */
        try
        {
            ep.libDestroy();
        }
        catch (Exception) { }

        /* Force delete Endpoint here, to avoid deletion from a non-
         * registered thread (by GC?). 
         */
        ep.Dispose();
        ep = null;
    }

    private void loadConfig(String filename)
    {
        try
        {
            JsonDocument json = new JsonDocument();
            /* Load file */
            json.loadFile(filename);
            ContainerNode root = json.getRootContainer();

            /* Read endpoint config */
            epConfig.readObject(root);

            /* Read transport config */
            ContainerNode tpNode = root.readContainer("SipTransport");
            sipTpConfig.readObject(tpNode);

            /* Read account config */
            ContainerNode accNode = root.readContainer("MyAccountConfig");
            myAccCfg.readObject(accNode);

            /* Force delete json now */
            json.Dispose();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private void buildAccConfigs()
    {
        MyAccountConfig tmpAccCfg = new MyAccountConfig();
        tmpAccCfg.accCfg = account.cfg;

        tmpAccCfg.buddyCfgs.Clear();
        for (int j = 0; j < account.buddyList.Count; j++)
        {
            MyBuddy bud = (MyBuddy)account.buddyList[j];
            tmpAccCfg.buddyCfgs.Add(bud.cfg);
        }

        myAccCfg = tmpAccCfg;
    }

    private void saveConfig(String filename)
    {
        try
        {
            JsonDocument json = new JsonDocument();

            /* Write endpoint config */
            json.writeObject(epConfig);

            /* Write transport config */
            ContainerNode tpNode = json.writeNewContainer("SipTransport");
            sipTpConfig.writeObject(tpNode);

            /* Write account configs */
            buildAccConfigs();
            ContainerNode accNode = json.writeNewContainer("MyAccountConfig");
            myAccCfg.writeObject(accNode);

            /* Save file */
            json.saveFile(filename);

            /* Force delete json now */
            json.Dispose();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}


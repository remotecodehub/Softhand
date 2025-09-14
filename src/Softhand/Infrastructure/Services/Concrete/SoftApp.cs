using pjsua2maui.pjsua2;

namespace Softhand.Infrastructure.Services.Concrete;
public class SoftApp(ILogger<SoftApp> _logger) : ISoftApp
{
    public static ISoftMonitor Monitor { get; set; }
    public static Endpoint Endpoint { get; set; } = new();
    public static SoftCall CurrentCall { get; set; } = null!;
    public static SoftAccount Account { get; set; }
    public static SoftConfig CurrentConfig { get; set; }
    public static CallInfo LastCallInfo { get; set; }
    public static SoftLogWriter LogWriter { get; set; } = new();
    public EpConfig EpConfig { get; set; } = new EpConfig();
    public TransportConfig SipTpConfig { get; set; } = new TransportConfig();
    public static AccountCallConfig AccountCallConfig { get; set; }
    public static AccountMediaConfig AccountMediaConfig { get; set; }
    public static AccountPresConfig AccountPresConfig { get; set; }
    public static AccountRegConfig AccountRegConfig { get; set; }
    public static AccountSipConfig AccountSipConfig { get; set; }
    public static AccountVideoConfig AccountVideoConfig { get; set; }
    public static CodecOpusConfig CodecOpusConfig { get; set; }
    public static MediaConfig MediaConfig { get; set; }
    public static CodecLyraConfig CodecLyraConfig { get; set; } 

    public string ConfigPath { get; set; } 

    private const string pjsipConfigFIle = "Softhand.json";
    private const int SIP_PORT = 5060;
    private const int LOG_LEVEL = 5;
     

    public void Init(ISoftMonitor monitor, string app_path)
    {
        _logger.LogInformation("Initing lib");
        Monitor = monitor; 
        /* Create endpoint */
        try
        {
            Dispatcher.GetForCurrentThread().Dispatch(Endpoint.libCreate);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error libCreate : {Message}", e.Message);
            return;
        }

        CurrentConfig = new SoftConfig();
        /* Load config */
        ConfigPath = System.IO.Path.Combine(app_path, pjsipConfigFIle);
        if (File.Exists(ConfigPath))
        {
            Dispatcher.GetForCurrentThread().Dispatch(() => LoadConfig(ConfigPath));
        }
        else
        {
            /* Set 'default' values */
            CurrentConfig.SipTpConfig.port = SIP_PORT;
        }

        /* Override log level setting */
        CurrentConfig.EpConfig.logConfig.level = LOG_LEVEL;
        CurrentConfig.EpConfig.logConfig.consoleLevel = LOG_LEVEL;

        /* Set log config. */
        LogConfig log_cfg = CurrentConfig.EpConfig.logConfig;
        LogWriter = new SoftLogWriter();
        log_cfg.writer = LogWriter;
        log_cfg.decor += (uint)pj_log_decoration.PJ_LOG_HAS_NEWLINE;

        UaConfig ua_cfg = CurrentConfig.EpConfig.uaConfig;
        ua_cfg.userAgent = "Softhand " + Endpoint.libVersion().full;

        /* Init endpoint */
        try
        {
            Dispatcher.GetForCurrentThread().Dispatch(() => Endpoint.libInit(EpConfig));
        }
        catch (Exception)
        {
            return;
        }

        /* Create transports. */
        try
        {
            Dispatcher.GetForCurrentThread().Dispatch(() => Endpoint.transportCreate(pjsip_transport_type_e.PJSIP_TRANSPORT_UDP,
                               SipTpConfig));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        try
        {
            Dispatcher.GetForCurrentThread().Dispatch(() => Endpoint.transportCreate(pjsip_transport_type_e.PJSIP_TRANSPORT_TCP,
                               SipTpConfig));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        try
        {
            SipTpConfig.port = SIP_PORT + 1;
            Dispatcher.GetForCurrentThread().Dispatch(() => Endpoint.transportCreate(pjsip_transport_type_e.PJSIP_TRANSPORT_TLS,
                               SipTpConfig));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        /* Set SIP port back to default for JSON saved config */
        SipTpConfig.port = SIP_PORT; 
        if (CurrentConfig.AccountConfig.idUri == "")
        {
            CurrentConfig.AccountConfig.idUri = "sip:localhost";
        }
        CurrentConfig.AccountConfig.natConfig.iceEnabled = true;
        CurrentConfig.AccountConfig.videoConfig.autoTransmitOutgoing = true;
        CurrentConfig.AccountConfig.videoConfig.autoShowIncoming = true;
        CurrentConfig.AccountConfig.mediaConfig.srtpUse = pjmedia_srtp_use.PJMEDIA_SRTP_OPTIONAL;
        CurrentConfig.AccountConfig.mediaConfig.srtpSecureSignaling = 0;

        Account = new SoftAccount(CurrentConfig.AccountConfig);
        try
        {
            Dispatcher.GetForCurrentThread().Dispatch(() =>
            {
                Account.create(CurrentConfig.AccountConfig);

                /* Add Buddies */
                foreach (BuddyConfig budCfg in CurrentConfig.BuddyConfigs)
                {
                    Account.AddBuddy(budCfg);
                }
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Account = null;
        }

        /* Start. */
        try
        {
            Dispatcher.GetForCurrentThread().Dispatch(Endpoint.libStart);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public void Deinit()
    {
        SaveConfig(ConfigPath);

        /* Shutdown pjsua. Note that Endpoint destructor will also invoke
         * libDestroy(), so this will be a test of double libDestroy().
         */
        try
        {
            Endpoint.libDestroy();
            /* Force delete Endpoint here, to avoid deletion from a non-
             * registered thread (by GC?). 
             */
            Endpoint.Dispose();
            Endpoint = null;
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
        }
    }

    public void LoadConfig(string filename)
    {
        JsonDocument json = new();
        try
        {
            /* Load file */
            json.loadFile(filename);
            ContainerNode root = json.getRootContainer();

            /* Read endpoint config */
            CurrentConfig.EpConfig.readObject(root);

            /* Read transport config */
            ContainerNode tpNode = root.readContainer("SipTransport");
            CurrentConfig.SipTpConfig.readObject(tpNode);

            /* Read Account config */
            ContainerNode accNode = root.readContainer("SoftConfig");
            CurrentConfig.AccountConfig.readObject(accNode);

            /* Force delete json now */
            json.Dispose();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public void BuildAccountConfigs()
    {
        SoftConfig tmpAccCfg = new();
        tmpAccCfg.AccountConfig = Account.Configuration;

        tmpAccCfg.BuddyConfigs.Clear();
        for (int j = 0; j < Account.BuddyList.Count; j++)
        {
            SoftBuddy bud = Account.BuddyList[j];
            tmpAccCfg.BuddyConfigs.Add(bud.Configuration);
        }

        CurrentConfig = tmpAccCfg;
    }

    public void SaveConfig(string filename)
    {
        try
        {
            JsonDocument json = new();

            /* Write endpoint config */
            json.writeObject(CurrentConfig.EpConfig);

            /* Write transport config */
            ContainerNode tpNode = json.writeNewContainer("SipTransport");
            CurrentConfig.SipTpConfig.writeObject(tpNode);

            /* Write Account configs */
            BuildAccountConfigs();
            ContainerNode accNode = json.writeNewContainer("SoftConfig");
            CurrentConfig.WriteObject(accNode);

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


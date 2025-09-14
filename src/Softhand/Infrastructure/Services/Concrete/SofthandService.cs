using pjsua2maui.pjsua2;

namespace Softhand.Infrastructure.Services.Concrete;
public class SofthandService(ILogger<SofthandService> _logger) : ISofthandService
{
    public static ISoftMonitor Monitor { get; set; }
    public Endpoint Endpoint { get; set; } = new();
    public SoftCall CurrentCall { get; set; } = null!;
    public SoftAccount Account { get; set; }
    public SoftConfig CurrentConfig { get; set; }
    public CallInfo LastCallInfo { get; set; }
    public SoftLogWriter LogWriter { get; set; } = new();
    public SoftAccountConfigModel SoftAccountConfig { get; set; }
    private string ConfigPath { get; set; }
    private static string ConfigFileName { get => "Softhand.json"; } 

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
        ConfigPath = System.IO.Path.Combine(app_path, ConfigFileName);
        if (File.Exists(ConfigPath))
        {
            Dispatcher.GetForCurrentThread().Dispatch(() => LoadConfig(ConfigPath));
        }
        else
        {
            /* Set 'default' values */
            CurrentConfig.SipTpConfig.port = 5060;
        }

        /* Override log level setting */
        CurrentConfig.EpConfig.logConfig.level = 5;
        CurrentConfig.EpConfig.logConfig.consoleLevel = 5;

        /* Set log config. */
        LogConfig log_cfg = CurrentConfig.EpConfig.logConfig;
        LogWriter = new SoftLogWriter();
        log_cfg.writer = LogWriter;
        log_cfg.decor += (uint)pj_log_decoration.PJ_LOG_HAS_NEWLINE;

        UaConfig ua_cfg = CurrentConfig.EpConfig.uaConfig;
        ua_cfg.userAgent = "Softhand:" + Endpoint.libVersion().full;

        

        /* Init endpoint */
        try
        {
            Dispatcher.GetForCurrentThread().Dispatch(() => Endpoint.libInit(CurrentConfig.EpConfig));
        }
        catch (Exception)
        {
            return;
        }

        /* Create transports. */
        try
        {
            Dispatcher.GetForCurrentThread().Dispatch(() => Endpoint.transportCreate(pjsip_transport_type_e.PJSIP_TRANSPORT_UDP,
                               CurrentConfig.SipTpConfig));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        try
        {
            Dispatcher.GetForCurrentThread().Dispatch(() => Endpoint.transportCreate(pjsip_transport_type_e.PJSIP_TRANSPORT_TCP,
                               CurrentConfig.SipTpConfig));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        try
        {
            CurrentConfig.SipTpConfig.port = 5061;
            Dispatcher.GetForCurrentThread().Dispatch(() => Endpoint.transportCreate(pjsip_transport_type_e.PJSIP_TRANSPORT_TLS,
                               CurrentConfig.SipTpConfig));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        /* Set SIP port back to default for JSON saved config */
        CurrentConfig.SipTpConfig.port = 5060; 
        if (CurrentConfig.AccountConfig.idUri == "")
        {
            CurrentConfig.AccountConfig.idUri = "0001";
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
        SoftConfig tmpAccCfg = new()
        {
            AccountConfig = Account.Configuration
        };

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


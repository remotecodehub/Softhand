namespace Softhand.Application.ViewModels;

public partial class BuddyViewModel : BaseViewModel, ISoftMonitor
{
    private readonly ILogger<BuddyViewModel> _logger;
    private readonly ISoftApp _softApp;
    private readonly INavigationContextService<SoftBuddy> _buddyContext;
     
    [ObservableProperty]
    private bool registered = false;

    [ObservableProperty]
    private string status = string.Empty;

    [ObservableProperty]
    private string settingsBtnTxt = string.Empty;

    [ObservableProperty]
    private string unregisterBtnTxt = string.Empty;

    [ObservableProperty]
    private SoftBuddy selectedBuddy = default!;

    [ObservableProperty]
    private ObservableCollection<SoftBuddy> buddies = [];

    public BuddyViewModel(ILogger<BuddyViewModel> logger, ISoftApp app, INavigationContextService<SoftBuddy> buddyContext)
    {
        Title = "Buddies";
        SettingsBtnTxt = "Settings";
        UnregisterBtnTxt = "Unregister";
        _logger = logger;
        _softApp = app; 
        _buddyContext = buddyContext; 

        try
        {
            string config_path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Dispatcher.GetForCurrentThread().Dispatch(() => _softApp.Init(this, config_path));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error _softApp.Init: {Message}", e.Message);
        }

        WeakReferenceMessenger.Default.Register<SaveAccountConfigMessage>(this, (r, m) =>
        {
            var myCfg = m.Value;
            SoftApp.CurrentConfig.AccountConfig.idUri = myCfg.IdUri;
            SoftApp.CurrentConfig.AccountConfig.regConfig.registrarUri = myCfg.RegistrarUri;
            SoftApp.CurrentConfig.AccountConfig.sipConfig.proxies.Clear();
            if (myCfg.Proxy != "")
            {
                SoftApp.CurrentConfig.AccountConfig.sipConfig.proxies.Add(myCfg.Proxy);
            }
            SoftApp.CurrentConfig.AccountConfig.sipConfig.authCreds.Clear();
            if (myCfg.Username != "" || myCfg.Password != "")
            {
                SoftApp.CurrentConfig.AccountConfig.sipConfig.authCreds.Add(new AuthCredInfo()
                {
                    username = myCfg.Username,
                    data = myCfg.Password,
                    realm = "*"
                });
            }

            try
            {
                SoftApp.Account.modify(SoftApp.CurrentConfig.AccountConfig);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        });

        WeakReferenceMessenger.Default.Register<AddBuddyMessage>(this, (r, m) =>
        {
            var budCfg = m.Value;
            try
            {
                SoftApp.Account.AddBuddy(budCfg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            this.LoadBuddiesCommand.Execute(null);
        });

        WeakReferenceMessenger.Default.Register<EditBuddyMessage>(this, (r, m) =>
        {
            var budCfg = m.Value;

            if (this.SelectedBuddy != null)
            {
                SoftApp.Account.DelBuddy(this.SelectedBuddy);
                try
                {
                    SoftApp.Account.AddBuddy(budCfg);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                this.LoadBuddiesCommand.Execute(null);
            }
        });

    }


    #region MONITOR
    public void notifyBuddyState(SoftBuddy buddy)
    {
        
    }

    public void notifyCallMediaState(SoftCall call)
    {
        if (SoftApp.CurrentCall == null || call.getId() != SoftApp.CurrentCall.getId())
            return;

        CallInfo ci = null;

        try
        {
            ci = call.getInfo();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        if (ci == null)
            return;
        WeakReferenceMessenger.Default.Send(new UpdateMediaCallStateMessage(ci));
    }

    public void notifyCallState(SoftCall call)
    {
        if (SoftApp.CurrentCall == null || call.getId() != SoftApp.CurrentCall.getId())
            return;

        CallInfo ci = null;
        try
        {
            ci = call.getInfo();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        if (ci == null)
            return;
        WeakReferenceMessenger.Default.Send(new UpdateCallStateMessage(ci));

        if (ci.state == pjsip_inv_state.PJSIP_INV_STATE_DISCONNECTED)
        {
            ThreadPool.QueueUserWorkItem(DeleteCall);
        }
    }

    public void notifyChangeNetwork()
    { 

    }

    public void notifyIncomingCall(SoftCall call)
    {
        CallOpParam prm = new CallOpParam();

        if (SoftApp.CurrentCall != null)
        {
            call.Dispose();
            return;
        }

        prm.statusCode = (pjsip_status_code.PJSIP_SC_RINGING);
        try
        {
            call.answer(prm);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        SoftApp.CurrentCall = call;
        Dispatcher.GetForCurrentThread().Dispatch(async () => { await Shell.Current.GoToAsync(new ShellNavigationState(Routes.CallPage), true); });
    }

    public void notifyRegState(int code, string reason, long expiration)
    {
        Registered = (code == (int)pjsip_status_code.PJSIP_SC_OK);
        _logger.LogInformation("Registration Code: {Code}", code);
        _logger.LogInformation("Registration Reason: {Reason}", reason);
        _logger.LogInformation("Registration Expiration: {Expiration}", expiration);

    }

    #endregion

    #region Commands
    [RelayCommand]
    public async Task Add()
    {
        IsBusy = true;  
        await Shell.Current.GoToAsync(new ShellNavigationState(Routes.BuddyConfigAddPage), true);
        IsBusy = false;
    }

    [RelayCommand]
    public void Delete()
    {
        IsBusy = true;
        if (this.SelectedBuddy != null)
        {
            SoftApp.Account.DelBuddy(this.SelectedBuddy);
            Buddies.Remove(SelectedBuddy);
            SelectedBuddy = null;
        }
        IsBusy = false;
    }

    [RelayCommand]
    public async Task Edit()
    {
        IsBusy = true;
        if (this.SelectedBuddy != null)
        {
            _buddyContext.Payload = this.SelectedBuddy;
            await Shell.Current.GoToAsync(new ShellNavigationState(Routes.BuddyConfigEditPage), true);
        }
        IsBusy = false;
    }

    [RelayCommand]
    private async Task Call()
    {
        IsBusy = true;
        if (this.SelectedBuddy != null)
        {
            SoftCall call = new(SoftApp.Account, -1);
            CallOpParam prm = new(true);

            try
            {
                await Dispatcher.GetForCurrentThread().DispatchAsync(() => call.makeCall(this.SelectedBuddy.cfg.uri, prm));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                call.Dispose();
                return;
            }
            SoftApp.CurrentCall = call;
            await Shell.Current.GoToAsync(new ShellNavigationState(Routes.CallPage), true);
        }
        IsBusy = false;
    }

    [RelayCommand]
    private void OnAppearing()
    {
        IsBusy = true;
        if (Buddies.Count == 0)
            this.LoadBuddiesCommand.Execute(null);
        IsBusy = false;
    }

    [RelayCommand]
    private void Unregister()
    {
        IsBusy = true;
        try
        {
            _softApp.Deinit();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
        IsBusy = false;
    }

    [RelayCommand]
    private async Task NavigateSettings()
    {
        IsBusy = true;
        await Shell.Current.GoToAsync(new ShellNavigationState(Routes.AccountConfigPage), true);
        IsBusy = false;
    }

    [RelayCommand]
    private void LoadBuddies()
    {
        if (IsBusy)
            return;

        IsBusy = true;

        try
        {
            Buddies.Clear();
            foreach (var buddy in SoftApp.Account.BuddyList)
            {
                Buddies.Add(buddy);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{Message}", e.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion

    #region Methods
    static void DeleteCall(Object stateInfo)
    {
        SoftApp.CurrentCall.Dispose();
        SoftApp.CurrentCall = null;
    }

    #endregion
}


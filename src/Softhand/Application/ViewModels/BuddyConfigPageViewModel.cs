namespace Softhand.Application.ViewModels;
[QueryProperty(nameof(IsEditMode), "isEditMode")]

public partial class BuddyConfigPageViewModel : BaseViewModel
{
    private readonly INavigationContextService<SoftBuddy> _buddyContext; 
    private readonly ILogger<BuddyConfigPageViewModel> _logger;
    
    [ObservableProperty]
    private bool isEditMode;
    
    [ObservableProperty]
    private bool subscribe;

    [ObservableProperty]
    private string uri = string.Empty;

    [ObservableProperty]
    private BuddyConfig _buddyConfig = default!;

    public BuddyConfigPageViewModel(ILogger<BuddyConfigPageViewModel> logger, INavigationContextService<SoftBuddy> buddyNavigationContext )
    { 
        this._buddyContext = buddyNavigationContext; 
        this._logger = logger;
        this.Title = !IsEditMode ? "Add Buddy" : "Edit Buddy";
        if (!IsEditMode)
        { 
            this.InitCommand.Execute(new BuddyConfig());
        }
        else
        {
            this.InitCommand.Execute(_buddyContext.Payload.Configuration);
        }
    }

    [RelayCommand]
    private void Init(BuddyConfig budCfg) 
    {
        this._logger.LogInformation("Initing Buddy Configuration");
        BuddyConfig = budCfg;
    }

    [RelayCommand]
    private void OnAppearing()
    {
        this.InitCommand.Execute(this._buddyContext.Payload.Configuration);
    }

    [RelayCommand]
    private async Task Ok()
    {
        if (!IsEditMode)
        {
            this._logger.LogInformation("Sending Add Buddy Message with : {Uri}", this.BuddyConfig.uri);
            WeakReferenceMessenger.Default.Send(new AddBuddyMessage(this.BuddyConfig));
        }
        else
        {
            this._logger.LogInformation("Sending Edit Buddy Message with : {Uri}", this.BuddyConfig.uri);
            WeakReferenceMessenger.Default.Send(new EditBuddyMessage(this.BuddyConfig));
        }
        await Shell.Current.GoToAsync(new ShellNavigationState(".."), true);
    }

    [RelayCommand]
    private static async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}


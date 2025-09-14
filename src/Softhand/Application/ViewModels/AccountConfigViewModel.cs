using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Softhand.Domain.Models;

namespace Softhand.Application.ViewModels;

/// <summary>
/// View model para página de configuração de conta sip
/// </summary>
/// <remarks>
/// Herda de <see cref="BaseViewModel"/>.
/// </remarks>
public partial class AccountConfigViewModel : BaseViewModel
{ 

    [ObservableProperty]
    private SoftAccountConfigModel accountConfig = default!;

    [RelayCommand]
    public void Init(SoftConfig inAccCfg)
    {
        this.AccountConfig = new SoftAccountConfigModel(inAccCfg);
    }

    [RelayCommand]
    private async Task Ok()
    {
        WeakReferenceMessenger.Default.Send(new SaveAccountConfigMessage(AccountConfig));
        await Shell.Current.Navigation.PopAsync(true);
    }

    [RelayCommand]
    private async static Task Cancel()
    {
        await Shell.Current.Navigation.PopAsync();
    }
}


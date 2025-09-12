using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Softhand.Models;

namespace Softhand.ViewModels;

public partial class BuddyViewModel : BaseViewModel
{
    private readonly ILogger<BuddyViewModel> _logger;

    [ObservableProperty]
    private SoftBuddy selectedBuddy = default!;

    [ObservableProperty]
    private ObservableCollection<SoftBuddy> buddies = [];

    public BuddyViewModel()
    {
        _logger = Microsoft.Maui.Controls.Application.Current.Handler.GetRequiredService<ILogger<BuddyViewModel>>();
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
            foreach (var buddy in SoftApp.account.BuddyList)
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
}


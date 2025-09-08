using System.Collections.ObjectModel;
using Softhand.Models;

namespace Softhand.ViewModels;

public class BuddyViewModel : BaseViewModel
{
    private SoftBuddy _selectedBuddy;
    public SoftBuddy SelectedBuddy {
        get { return _selectedBuddy; }
        set { SetProperty(ref _selectedBuddy, value); }
    }
    public ObservableCollection<SoftBuddy> Buddies { get; set; }
    public Command LoadBuddiesCommand { get; set; }

    public BuddyViewModel()
    {
        Buddies = new ObservableCollection<SoftBuddy>();
        LoadBuddiesCommand = new Command(() => ExecuteLoadBuddiesCommand());
    }

    void ExecuteLoadBuddiesCommand()
    {
        if (IsBusy)
            return;

        IsBusy = true;

        try {
            Buddies.Clear();
            foreach (var buddy in SoftApp.account.BuddyList) {
                Buddies.Add(buddy);
            }
        } catch (Exception e) {
            Console.WriteLine(e.Message);
        } finally {
            IsBusy = false;
        }
    }
}


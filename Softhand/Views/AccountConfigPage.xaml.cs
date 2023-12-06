using CommunityToolkit.Mvvm.Messaging;
using pjsua2xamarin.pjsua2;
using Softhand.Messages;
using Softhand.Models;
using Softhand.ViewModels;

namespace Softhand.Views
{
    public partial class AccountConfigPage : ContentPage
    {
        AccountConfigViewModel viewModel;

        public AccountConfigPage(SoftAccountConfig accCfg)
        {
            InitializeComponent();

            viewModel = new AccountConfigViewModel();
            viewModel.init(accCfg);
            BindingContext = viewModel;
        }

        async void Ok_Clicked(object sender, EventArgs e)
        {
            WeakReferenceMessenger.Default.Send(new SaveAccountConfigMessage(viewModel.accCfg));
            await Navigation.PopAsync();
        }

        async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}


using pjsua2xamarin.pjsua2;
using Softhand.Models;
using Softhand.ViewModels;

namespace Softhand.Views
{
    public partial class AVConfigPage : ContentPage
    {
        AVConfigViewModel viewModel;

        public AVConfigPage(SoftAccountConfig accCfg)
        {
            InitializeComponent();

            viewModel = new AVConfigViewModel();
            viewModel.init(accCfg);
            BindingContext = viewModel;
        }

        async void Ok_Clicked(object sender, EventArgs e)
        {
            //MessagingCenter.Send(this, "SaveAccountConfig", viewModel.accCfg);
            await Navigation.PopAsync();
        }

        async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}


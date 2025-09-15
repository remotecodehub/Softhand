namespace Softhand.Application.Views;

public partial class AccountConfigPage : ContentPage
{
    public AccountConfigPage(AccountConfigPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        viewModel.Init(SoftApp.CurrentConfig);
    }
}


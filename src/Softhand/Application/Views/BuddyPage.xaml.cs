namespace Softhand.Application.Views;

public partial class BuddyPage : ContentPage
{ 
    public BuddyPage(BuddyPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    } 
}


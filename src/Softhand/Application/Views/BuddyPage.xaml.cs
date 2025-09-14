namespace Softhand.Application.Views;

public partial class BuddyPage : ContentPage
{ 
    public BuddyPage(BuddyViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    } 
}


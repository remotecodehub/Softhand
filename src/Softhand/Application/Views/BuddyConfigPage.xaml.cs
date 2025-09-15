namespace Softhand.Application.Views
{
    public partial class BuddyConfigPage : ContentPage
    { 
        public BuddyConfigPage(BuddyConfigPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;   
        }
    }
}


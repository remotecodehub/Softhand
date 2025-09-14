namespace Softhand.Application.Views
{
    public partial class BuddyConfigPage : ContentPage
    { 
        public BuddyConfigPage(BuddyConfigViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;   
        }
    }
}


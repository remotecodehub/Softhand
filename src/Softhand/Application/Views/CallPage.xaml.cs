namespace Softhand.Application.Views;

public partial class CallPage : ContentPage
{    
    public CallPage (CallPageViewModel viewModel)
    {
        InitializeComponent ();
        BindingContext = viewModel; 
    }
}


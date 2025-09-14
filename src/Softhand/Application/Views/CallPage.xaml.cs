namespace Softhand.Application.Views;

public partial class CallPage : ContentPage
{    
    public CallPage (CallViewModel viewModel)
    {
        InitializeComponent ();
        BindingContext = viewModel; 
    }
}


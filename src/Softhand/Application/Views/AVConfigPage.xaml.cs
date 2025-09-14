using pjsua2maui.pjsua2;
using Softhand.Domain.Models;
using Softhand.Application.ViewModels;

namespace Softhand.Application.Views;

public partial class AVConfigPage : ContentPage
{
    public AVConfigPage(AVConfigViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}


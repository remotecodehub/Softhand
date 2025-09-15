namespace Softhand.Application.ViewModels;

public partial class AVConfigPageViewModel : BaseViewModel
{

    [ObservableProperty]
    private string cancelBtnTxt = string.Empty;

    [ObservableProperty]
    private string okBtnTxt = string.Empty;

    [ObservableProperty]
    private SoftAccountConfigModel accountConfig = default!;
    
    [RelayCommand]
    private void Init(SoftConfig inAccCfg = null)
    {
        OkBtnTxt = "Save";
        CancelBtnTxt = "Cancel";
        Title = "Audio & Video";
        this.AccountConfig = new SoftAccountConfigModel(inAccCfg);
    }
    [RelayCommand]
    private void Save()
    {
        throw new NotImplementedException();
    }
    [RelayCommand]
    private void Cancel()
    {
        throw new NotImplementedException();
    }
}


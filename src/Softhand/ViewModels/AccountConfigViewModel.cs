namespace Softhand.ViewModels;

public class AccountConfigViewModel : BaseViewModel
{
    public SoftAccountConfigModel AccountConfig { get; set; }

    public void Init(SoftAccountConfig inAccCfg = null)
    {
        AccountConfig = new SoftAccountConfigModel(inAccCfg);
    }
}


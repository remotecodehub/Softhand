using Softhand.Models;

namespace Softhand.ViewModels;

public class AccountConfigViewModel : BaseViewModel
{
    public SoftAccountConfigModel accCfg { get; set; }

    public AccountConfigViewModel()
    {
        
    }

    public void init(SoftAccountConfig inAccCfg = null)
    {
        accCfg = new SoftAccountConfigModel(inAccCfg);
    }
}


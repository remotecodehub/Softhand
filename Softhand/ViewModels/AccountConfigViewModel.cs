using Softhand.Models;

namespace Softhand.ViewModels;

public class AccountConfigViewModel : BaseViewModel
{
    public MyAccountConfigModel accCfg { get; set; }

    public AccountConfigViewModel()
    {
        
    }

    public void init(MyAccountConfig inAccCfg = null)
    {
        accCfg = new MyAccountConfigModel(inAccCfg);
    }
}


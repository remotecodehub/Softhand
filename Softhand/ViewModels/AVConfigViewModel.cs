using Softhand.Models;

namespace Softhand.ViewModels;

public class AVConfigViewModel : BaseViewModel
{
    public SoftAccountConfigModel accCfg { get; set; }

    public AVConfigViewModel()
    {
        
    }

    public void init(SoftAccountConfig inAccCfg = null)
    {
        accCfg = new SoftAccountConfigModel(inAccCfg);
    }
}


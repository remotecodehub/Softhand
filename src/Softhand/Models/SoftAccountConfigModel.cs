using pjsua2maui.pjsua2;

namespace Softhand.Models;

public class SoftAccountConfigModel
{
    public string IdUri { get; set; } 
    public string RegistrarUri { get; set; }
    public string Proxy { get; set; }
    public string Username { get; set; }
    public string Password { get; set; } 

    public SoftAccountConfigModel(SoftAccountConfig inAccConfig)
    {
        AccountConfig accCfg = inAccConfig.accCfg;

        IdUri = accCfg.idUri;
        RegistrarUri = accCfg.regConfig.registrarUri;
        if (accCfg.sipConfig.proxies.Count > 0)
            Proxy = accCfg.sipConfig.proxies[0];
        else
            Proxy = "";

        if (accCfg.sipConfig.authCreds.Count > 0) {
            Username = accCfg.sipConfig.authCreds[0].username;
            Password = accCfg.sipConfig.authCreds[0].data;
        } else {
            Username = "";
            Password = "";
        }
    }
}


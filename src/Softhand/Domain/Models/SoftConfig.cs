using pjsua2maui.pjsua2;

namespace Softhand.Domain.Models;

public class SoftConfig
{
    public TransportConfig SipTpConfig { get; set; } = new TransportConfig();
    public AccountConfig AccountConfig { get; set; }
    public EpConfig EpConfig { get; set; } = new EpConfig();
    public List<BuddyConfig> BuddyConfigs { get; set; } = [];

    public SoftConfig()
    {
        AccountConfig = new AccountConfig();
    }

    public void ReadObject(ContainerNode accNode)
    {
        try
        {
            AccountConfig.readObject(accNode);
            ContainerNode buddiesNode = accNode.readArray("Buddies");
            BuddyConfigs.Clear();
            while (buddiesNode.hasUnread())
            {
                BuddyConfig budCfg = new BuddyConfig();
                budCfg.readObject(buddiesNode);
                BuddyConfigs.Add(budCfg);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public void WriteObject(ContainerNode accNode)
    {
        try
        {
            AccountConfig.writeObject(accNode);
            ContainerNode buddiesNode = accNode.writeNewArray("Buddies");
            foreach (BuddyConfig budCfg in BuddyConfigs)
            {
                budCfg.writeObject(buddiesNode);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
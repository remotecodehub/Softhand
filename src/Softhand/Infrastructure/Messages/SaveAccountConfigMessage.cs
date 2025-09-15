namespace Softhand.Infrastructure.Messages;

public class SaveAccountConfigMessage(SoftAccountConfigModel softAccountConfigModel) : ValueChangedMessage<SoftAccountConfigModel>(softAccountConfigModel)
{
}
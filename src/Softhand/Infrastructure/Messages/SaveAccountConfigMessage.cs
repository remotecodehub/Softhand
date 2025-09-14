using CommunityToolkit.Mvvm.Messaging.Messages;
using Softhand.Domain.Models;

namespace Softhand.Infrastructure.Messages;

public class SaveAccountConfigMessage(SoftAccountConfigModel softAccountConfigModel) : ValueChangedMessage<SoftAccountConfigModel>(softAccountConfigModel)
{
}
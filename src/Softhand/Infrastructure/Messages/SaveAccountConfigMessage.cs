using CommunityToolkit.Mvvm.Messaging.Messages;
using Softhand.Domain.Models;

namespace Softhand.Infrastructure.Messages;

public class SaveAccountConfigMessage : ValueChangedMessage<SoftAccountConfigModel>
{
	public SaveAccountConfigMessage(SoftAccountConfigModel softAccountConfigModel) : base (softAccountConfigModel)
	{
	}
}
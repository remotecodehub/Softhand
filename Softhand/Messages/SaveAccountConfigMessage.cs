using CommunityToolkit.Mvvm.Messaging.Messages;
using Softhand.Models;

namespace Softhand.Messages
{
	public class SaveAccountConfigMessage : ValueChangedMessage<SoftAccountConfigModel>
	{
		public SaveAccountConfigMessage(SoftAccountConfigModel softAccountConfigModel) : base (softAccountConfigModel)
		{
		}
	}
}
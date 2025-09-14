using System;
using CommunityToolkit.Mvvm.Messaging.Messages;
using pjsua2maui.pjsua2;

namespace Softhand.Infrastructure.Messages
{
	public class EditBuddyMessage : ValueChangedMessage<BuddyConfig>
    {
		public EditBuddyMessage(BuddyConfig buddyConfig) : base (buddyConfig)
		{
		}
	}
}


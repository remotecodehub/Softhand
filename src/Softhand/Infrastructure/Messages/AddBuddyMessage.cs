using System;
using CommunityToolkit.Mvvm.Messaging.Messages;
using pjsua2maui.pjsua2;

namespace Softhand.Infrastructure.Messages
{
	public class AddBuddyMessage : ValueChangedMessage<BuddyConfig>
    {
		public AddBuddyMessage(BuddyConfig buddyConfig) : base (buddyConfig)
		{
		}
	}
}


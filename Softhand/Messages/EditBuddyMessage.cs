using System;
using CommunityToolkit.Mvvm.Messaging.Messages;
using pjsua2xamarin.pjsua2;

namespace Softhand.Messages
{
	public class EditBuddyMessage : ValueChangedMessage<BuddyConfig>
    {
		public EditBuddyMessage(BuddyConfig buddyConfig) : base (buddyConfig)
		{
		}
	}
}


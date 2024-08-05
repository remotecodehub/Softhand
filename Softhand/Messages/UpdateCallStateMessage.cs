using System;
using CommunityToolkit.Mvvm.Messaging.Messages;
using pjsua2maui.pjsua2;

namespace Softhand.Messages
{
	public class UpdateCallStateMessage : ValueChangedMessage<CallInfo>
    {
		public UpdateCallStateMessage(CallInfo callInfo) : base (callInfo)
		{
		}
	}
}


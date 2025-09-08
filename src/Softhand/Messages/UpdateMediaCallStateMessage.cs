﻿using System;
using CommunityToolkit.Mvvm.Messaging.Messages;
using pjsua2maui.pjsua2;

namespace Softhand.Messages
{
	public class UpdateMediaCallStateMessage : ValueChangedMessage<CallInfo>
    {
		public UpdateMediaCallStateMessage(CallInfo callInfo) : base (callInfo)
		{
		}
	}
}


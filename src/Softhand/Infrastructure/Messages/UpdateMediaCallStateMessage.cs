namespace Softhand.Infrastructure.Messages;

public class UpdateMediaCallStateMessage(CallInfo callInfo) : ValueChangedMessage<CallInfo>(callInfo)
{
}


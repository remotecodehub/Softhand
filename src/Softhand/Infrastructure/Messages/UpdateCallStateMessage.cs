namespace Softhand.Infrastructure.Messages;

public class UpdateCallStateMessage(CallInfo callInfo) : ValueChangedMessage<CallInfo>(callInfo)
{
}


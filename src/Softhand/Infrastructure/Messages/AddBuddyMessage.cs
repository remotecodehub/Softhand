namespace Softhand.Infrastructure.Messages;
public class AddBuddyMessage(BuddyConfig buddyConfig) : ValueChangedMessage<BuddyConfig>(buddyConfig)
{
}


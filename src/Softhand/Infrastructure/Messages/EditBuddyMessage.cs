namespace Softhand.Infrastructure.Messages;
public class EditBuddyMessage(BuddyConfig buddyConfig) : ValueChangedMessage<BuddyConfig>(buddyConfig)
{
}


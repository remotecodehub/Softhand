namespace Softhand.Infrastructure.Messages;

public class InitSelectedBuddyConfigMessage(SoftBuddy buddy) : ValueChangedMessage<SoftBuddy>(buddy)
{
}

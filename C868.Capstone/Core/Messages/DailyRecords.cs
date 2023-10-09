using CommunityToolkit.Mvvm.Messaging.Messages;

namespace C868.Capstone.Core.Messages
{
    public class OpenDayMessage : ValueChangedMessage<bool>
    {
        public OpenDayMessage(bool value) : base(value)
        {
        }
    }

    public class CloseDayMessage : ValueChangedMessage<bool>
    {
        public CloseDayMessage(bool value) : base(value)
        {
        }
    }
}
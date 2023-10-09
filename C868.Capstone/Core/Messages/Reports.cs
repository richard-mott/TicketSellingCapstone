using CommunityToolkit.Mvvm.Messaging.Messages;

namespace C868.Capstone.Core.Messages
{
    public class ShowReportsViewMessage : ValueChangedMessage<bool>
    {
        public ShowReportsViewMessage(bool value) : base(value)
        {
        }
    }
}

using CommunityToolkit.Mvvm.Messaging.Messages;

namespace C868.Capstone.Core.Messages
{
    public class ShowSellingViewMessage : ValueChangedMessage<bool>
    {
        public ShowSellingViewMessage(bool value) : base(value)
        {
        }
    }
}

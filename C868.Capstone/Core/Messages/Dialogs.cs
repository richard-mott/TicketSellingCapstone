using CommunityToolkit.Mvvm.Messaging.Messages;

namespace C868.Capstone.Core.Messages
{
    public class DialogResultMessage : ValueChangedMessage<bool?>
    {
        public DialogResultMessage(bool? value) : base(value)
        {
        }
    }
}
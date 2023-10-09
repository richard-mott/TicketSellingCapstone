using C868.Capstone.Core.ViewModels.Data;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace C868.Capstone.Core.Messages
{
    public class ShowShowTimesViewMessage : ValueChangedMessage<bool>
    {
        public ShowShowTimesViewMessage(bool value) : base(value)
        {
        }
    }

    public class SelectedShowTimeChangedMessage : ValueChangedMessage<ShowTimeViewModel>
    {
        public SelectedShowTimeChangedMessage(ShowTimeViewModel value) : base(value)
        {
        }
    }

    public class ShowTimeSavedMessage : ValueChangedMessage<ShowTimeViewModel>
    {
        public ShowTimeSavedMessage(ShowTimeViewModel value) : base(value)
        {
        }
    }

    public class ShowTimeDeletedMessage : ValueChangedMessage<ShowTimeViewModel>
    {
        public ShowTimeDeletedMessage(ShowTimeViewModel value) : base(value)
        {
        }
    }
}

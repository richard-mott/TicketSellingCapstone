using C868.Capstone.Core.ViewModels.Data;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace C868.Capstone.Core.Messages
{
    public class ShowTicketTypesViewMessage : ValueChangedMessage<bool>
    {
        public ShowTicketTypesViewMessage(bool value) : base(value)
        {
        }
    }

    public class SelectedTicketTypeChangedMessage : ValueChangedMessage<TicketTypeViewModel>
    {
        public SelectedTicketTypeChangedMessage(TicketTypeViewModel value) : base(value)
        {
        }
    }

    public class ClearSelectedTicketTypeMessage : ValueChangedMessage<bool>
    {
        public ClearSelectedTicketTypeMessage(bool value) : base(value)
        {
        }
    }

    public class TicketTypeSavedMessage : ValueChangedMessage<TicketTypeViewModel>
    {
        public TicketTypeSavedMessage(TicketTypeViewModel value) : base(value)
        {
        }
    }
}
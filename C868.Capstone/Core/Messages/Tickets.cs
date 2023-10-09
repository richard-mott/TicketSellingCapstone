using System.Collections.Generic;
using C868.Capstone.Core.ViewModels.Data;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace C868.Capstone.Core.Messages
{
    public class TicketAddedMessage : ValueChangedMessage<TicketViewModel>
    {
        public TicketAddedMessage(TicketViewModel value) : base(value)
        {
        }
    }

    public class TransactionCompleteMessage : ValueChangedMessage<Dictionary<int, int>>
    {
        public TransactionCompleteMessage(Dictionary<int, int> value) : base(value)
        {
        }
    }
}
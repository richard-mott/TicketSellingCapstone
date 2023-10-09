using C868.Capstone.Core.ViewModels.Data;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace C868.Capstone.Core.Messages
{
    public class ShowAuditoriumsViewMessage : ValueChangedMessage<bool>
    {
        public ShowAuditoriumsViewMessage(bool value) : base(value)
        {
        }
    }

    public class SelectedAuditoriumChangedMessage : ValueChangedMessage<AuditoriumViewModel>
    {
        public SelectedAuditoriumChangedMessage(AuditoriumViewModel value) : base(value)
        {
        }
    }

    public class ClearSelectedAuditoriumMessage : ValueChangedMessage<bool>
    {
        public ClearSelectedAuditoriumMessage(bool value) : base(value)
        {
        }
    }

    public class AuditoriumSavedMessage : ValueChangedMessage<AuditoriumViewModel>
    {
        public AuditoriumSavedMessage(AuditoriumViewModel value) : base(value)
        {
        }
    }
}
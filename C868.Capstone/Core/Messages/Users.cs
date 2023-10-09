using C868.Capstone.Core.Models.Data;
using C868.Capstone.Core.ViewModels.Data;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace C868.Capstone.Core.Messages
{
    public class ShowUsersViewMessage : ValueChangedMessage<bool>
    {
        public ShowUsersViewMessage(bool value) : base(value)
        {
        }
    }

    public class LoggedInUserChangedMessage : ValueChangedMessage<User>
    {
        public LoggedInUserChangedMessage(User value) : base(value)
        {
        }
    }

    public class SelectedUserChangedMessage : ValueChangedMessage<UserViewModel>
    {
        public SelectedUserChangedMessage(UserViewModel value) : base(value)
        {
        }
    }

    public class ClearSelectedUserMessage : ValueChangedMessage<bool>
    {
        public ClearSelectedUserMessage(bool value) : base(value)
        {
        }
    }

    public class UserSavedMessage : ValueChangedMessage<UserViewModel>
    {
        public UserSavedMessage(UserViewModel value) : base(value)
        {
        }
    }

    public class ClearPasswordsMessage : ValueChangedMessage<bool>
    {
        public ClearPasswordsMessage(bool value) : base(value)
        {
        }
    }
}

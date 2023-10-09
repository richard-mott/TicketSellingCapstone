using C868.Capstone.Core.ViewModels.Data;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace C868.Capstone.Core.Messages
{
    public class ShowMoviesViewMessage : ValueChangedMessage<bool>
    {
        public ShowMoviesViewMessage(bool value) : base(value)
        {
        }
    }

    public class SelectedMovieChangedMessage : ValueChangedMessage<MovieViewModel>
    {
        public SelectedMovieChangedMessage(MovieViewModel value) : base(value)
        {
        }
    }

    public class ClearSelectedMovieMessage : ValueChangedMessage<bool>
    {
        public ClearSelectedMovieMessage(bool value) : base(value)
        {
        }
    }

    public class MovieSavedMessage : ValueChangedMessage<MovieViewModel>
    {
        public MovieSavedMessage(MovieViewModel value) : base(value)
        {
        }
    }
}

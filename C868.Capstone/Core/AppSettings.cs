namespace C868.Capstone.Core
{
    public static class AppSettings
    { 
        public static class Captions
        {
            public const string Auditoriums = @"Auditoriums";
            public const string Close = @"Close Day";
            public const string LogIn = @"Log In";
            public const string LogOut = @"Log Out";
            public const string Movies = @"Movies";
            public const string Open = @"Open Day";
            public const string Reports = @"Reports";
            public const string Selling = @"Selling";
            public const string ShowTimes = @"Show Times";
            public const string TicketTypes = @"Ticket Types";
            public const string Users = @"Users";
        }

        public static class Icons
        {
            public const string Auditoriums = @"/Icons/Auditoriums.png"; 
            public const string Close = @"/Icons/Close.png";
            public const string Error = @"/Icons/Error.png";
            public const string LogIn = @"/Icons/LogIn.png";
            public const string LogOut = @"/Icons/LogOut.png";
            public const string Movies = @"/Icons/Movies.png";
            public const string Open = @"/Icons/Open.png";
            public const string Reports = @"/Icons/Reports.png";
            public const string Selling = @"/Icons/Selling.png";
            public const string ShowTimes = @"/Icons/ShowTimes.png";
            public const string TicketTypes = @"/Icons/TicketTypes.png";
            public const string Users = @"/Icons/Users.png";
            public const string Warning = @"/Icons/Warning.png";
        }

        public static class Dialogs
        {
            public const string LogInTitle = @"Log In";

            public const string OverlappingShowTitle = @"Overlapping Show Times";
            public const string OverlappingShowMessage =
                @"The selected time overlaps with the following show";

            public const string LastAdminTitle = @"Cannot Delete Only Administrator";
            public const string LastAdminMessage =
                "You cannot delete the only Administrator account. " +
                "Please add another Administrator and try again.";
        }

        public static class Default
        {
            public static class Database
            {
                public const string FileName = @"tix.db";
            }

            public static class Times
            {
                public const int MatineeStartHour = 10;
                public const int MatineeStartMinute = 0;
                public const int MatineeStartSecond = 0;

                public const int MatineeEndHour = 17;
                public const int MatineeEndMinute = 45;
                public const int MatineeEndSecond = 0;
            }
        }

        public static class Errors
        {
            public static class ContentPage
            {
                public const string Title = @"Error Loading Content";
                public const string Message =
                    @"There was an error loading the page content. " +
                    @"Please try again or contact your system administrator for " +
                    @"more information.";
            }

            public static class Data
            {
                public const string LoadTitle = @"Error Loading Record";
                public const string LoadMessage =
                    @"There was an error loading the record from the database. " +
                    @"Please try again or contact your system adminitrator for " +
                    @"more information.";

                public const string SaveTitle = @"Error Saving Record";
                public const string SaveMessage =
                    @"There was an error saving the record to the database. " +
                    @"Please try again or contact your system adminitrator for " +
                    @"more information.";

                public const string DeleteTitle = @"Error Deleting Record";
                public const string DeleteMessage =
                    @"There was an error deleting the record from the database. " +
                    @"Please try again or contact your system adminitrator for " +
                    @"more information.";
            }
        }

        public static class ValidationErrors
        {

            public static class LogIn
            {
                public const string BlankUserName = @"Please enter a user name.";
                public const string BlankPassword = @"Please enter a password.";
                public const string IncorrectLogIn = @"User name or password is incorrect.";
            }

            public static class User
            {
                public const string UserNameBlank = @"Please enter a user name.";
                public const string UserNameTaken = @"That user name is already in use. Please enter a new one.";
                public const string CurrentPasswordBlank = @"Please enter the current password.";
                public const string CurrentPasswordInvalid = @"The current password is not correct.";
                public const string NewPasswordBlank = @"Please enter a new password.";
                public const string ConfirmPasswordBlank = @"Please confirm your password.";
                public const string PasswordsDoNotMatch = @"Your entered passwords do not match.";
            }

            public static class Auditorium
            {
                public const string NameBlank = @"Please enter a name.";
                public const string CapacityLessThanOne = @"Capacity must be greater than zero.";
            }

            public static class Movie
            {
                public const string NameBlank = @"Please enter a name.";
                public const string RatingBlank = @"Please select a rating";
                public const string RunTimeLessThanOne = @"Run time (in minutes) must be greater than zero.";
                public const string CastBlank = @"Please enter at least one cast member.";
                public const string DescriptionBlank = @"Please enter a description.";
            }

            public static class ShowTime
            {
                public const string AuditoriumBlank = @"Please select an auditorium.";
                public const string StartTimeBlank = @"Please enter a start time.";
            }

            public static class TicketType
            {
                public const string NameBlank = @"Please enter a name.";
                public const string PriceBelowZero = @"Price must be greater than zero.";
                public const string TimeTypeBlank = @"Please select a time type.";
                public const string StartTimeAfterEnd = @"The start time must be before the end time.";
                public const string RatingsBlank = @"Please select at least one rating.";
            }
        }

        public static class Schedule
        {
            public const double RowHeight = 50d;
            public const double AuditoriumWidth = 400;
            public const double TimeWidth = 200;

            public const string AuditoriumTitle = @"Auditoriums";

            // Resources
            public const string AuditoriumBackground = @"Brush-Vintage-Yellow";
            public const string HeaderBackground = @"Brush-Vintage-LightBlue";
            public const string ShowTimeBackground = @"Brush-Vintage-LightGreen";
            public const string ShowTimeBorderBrush = @"Brush-Vintage-Black";
            public const string SelectedShowTimeBackground = @"Brush-Vintage-Red";
            public const string SeparatorBorderBrush = @"Brush-Vintage-Black";

            public const string BorderThicknessSmall = @"Border-Thickness-Small";
            public const string BorderThicknessNormal = @"Border-Thickness-Normal";
            public const string FontSizeNormal = @"Font-Size-Normal";
            public const string MarginNone = @"Margin-None";
            public const string PaddingNone = @"Padding-None";
            public const string PaddingNormal = @"Padding-Normal";
            public const string PaddingLeftIndent = @"Padding-Normal-LeftIndent";
            public const string PaddingRightIndent = @"Padding-Normal-RightIndent";

            public const double SeparatorOpacity = 0.5d;
        }
    }
}

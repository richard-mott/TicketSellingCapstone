using System;

namespace C868.Capstone.Core
{
    public enum ErrorType
    {
        Error = 0,
        Warning = 1
    }

    public enum LogMessageType
    {
        None = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }

    [Flags]
    public enum UserType
    {
        User = 0,
        Administrator = 1
    }

    [Flags]
    public enum Rating
    {
        None = 0,
        G = 1,
        PG = 2,
        PG13 = 4,
        R = 8,
        NC17 = 16
    }

    public enum TicketTimeType
    {
        None = 0,
        Before = 1,
        During = 2,
        After = 3
    }

    public enum PaymentType
    {
        None = 0,
        Cash = 1,
        Credit = 2,
        Check = 3
    }

    public enum TextIndent
    {
        None = 0,
        Left = 1,
        Right = 2
    }

    public enum SeparatorOrientation
    {
        Horizontal = 0,
        Vertical = 1
    }
}

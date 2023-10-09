using C868.Capstone.Core.Models.Data;

namespace C868.Capstone.Services.Logging
{
    public interface ILoggingService
    {
        User User { get; set; }

        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
    }
}
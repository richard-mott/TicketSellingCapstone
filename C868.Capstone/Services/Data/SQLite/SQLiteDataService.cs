using System;
using System.IO;
using System.Threading.Tasks;
using C868.Capstone.Core;
using C868.Capstone.Core.Models.Activities;
using C868.Capstone.Core.Models.Data;
using C868.Capstone.Services.Data.Sample;
using SQLite;
using Microsoft.Extensions.DependencyInjection;

namespace C868.Capstone.Services.Data.SQLite
{
    public partial class SQLiteDataService : IDataService
    {
        private SampleDataService sampleDataService;
        private readonly SQLiteAsyncConnection dbContext;
        private readonly IServiceProvider services = App.Current.Services;

        public SQLiteDataService()
        {
            var databasePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                AppSettings.Default.Database.FileName);
            
            dbContext = new SQLiteAsyncConnection(databasePath);
        }

        public async Task InitializeTables(IProgress<int> valueProgress,
            IProgress<string> descriptionProgress)
        {
            await dbContext.CreateTableAsync<User>();
            await dbContext.CreateTableAsync<Auditorium>();
            await dbContext.CreateTableAsync<Movie>();
            await dbContext.CreateTableAsync<TicketType>();
            await dbContext.CreateTableAsync<ShowTime>();
            await dbContext.CreateTableAsync<Ticket>();
            await dbContext.CreateTableAsync<LogEntry>();
            await dbContext.CreateTableAsync<DailyRecord>();

            // Always create the initial two users, if necessary
            if (!await HasUsersAsync())
            {
                await InitializeSampleData();
                await InitializeUsers();
            }
        }

        public async Task InitializeData(IProgress<int> valueProgress,
            IProgress<string> descriptionProgress)
        {
            await InitializeSampleData();

            if (!await HasAuditoriumsAsync())
            {
                await InitializeAuditoriums(valueProgress, descriptionProgress);
            }

            if (!await HasMoviesAsync())
            {
                await InitializeMovies(valueProgress, descriptionProgress);
            }

            if (!await HasTicketTypesAsync())
            {
                await InitializeTicketTypes(valueProgress, descriptionProgress);
            }

            if (!await HasShowTimesAsync())
            {
                await InitializeShowTimes(valueProgress, descriptionProgress);
            }

            if (!await HasTicketsAsync())
            {
                await InitializeTickets(valueProgress, descriptionProgress);
            }

            if (!await HasLogEntriesAsync())
            {
                await InitializeLogEntries(valueProgress, descriptionProgress);
            }

            if (!await HasDailyRecordsAsync())
            {
                await InitializeDailyActivities(valueProgress, descriptionProgress);
            }
        }

        private async Task InitializeSampleData()
        {
            if (sampleDataService != null)
            {
                return;
            }

            sampleDataService = services.GetService<SampleDataService>();

            if (sampleDataService is null)
            {
                throw new NullReferenceException(@"Unable to initialize the sample data.");
            }

            await sampleDataService.InitializeTables(null, null);
            await sampleDataService.InitializeData(null, null);
        }

        private async Task InitializeUsers()
        {
            var sampleUsers = await sampleDataService.GetUsersAsync();

            foreach (var user in sampleUsers)
            {
                if (!await SaveUserAsync(user))
                {
                    throw new InvalidOperationException($"Failed to save user: {user.UserName}.");
                }
            }
        }

        private async Task InitializeAuditoriums(IProgress<int> valueProgress,
            IProgress<string> descriptionProgress)
        {
            descriptionProgress.Report(@"Creating sample auditoriums...");
            valueProgress.Report(0);

            var sampleAuditoriums = await sampleDataService.GetAuditoriumsAsync();

            for (var index = 0; index < sampleAuditoriums.Count; index++)
            {
                var auditorium = sampleAuditoriums[index];

                if (!await SaveAuditoriumAsync(auditorium))
                {
                    throw new InvalidOperationException($"Failed to save auditorium: {auditorium.Name}.");
                }

                valueProgress.Report((index + 1) * 100 / sampleAuditoriums.Count);
            }

            descriptionProgress.Report(string.Empty);
        }

        private async Task InitializeMovies(IProgress<int> valueProgress,
            IProgress<string> descriptionProgress)
        {
            descriptionProgress.Report(@"Creating sample movies...");
            valueProgress.Report(0);

            var sampleMovies = await sampleDataService.GetMoviesAsync();

            for (var index = 0; index < sampleMovies.Count; index++)
            {
                var movie = sampleMovies[index];

                if (!await SaveMovieAsync(movie))
                {
                    throw new InvalidOperationException($"Failed to save movie: {movie.Name}.");
                }

                valueProgress.Report((index + 1) * 100 / sampleMovies.Count);
            }

            descriptionProgress.Report(string.Empty);
        }

        private async Task InitializeTicketTypes(IProgress<int> valueProgress,
            IProgress<string> descriptionProgress)
        {
            descriptionProgress.Report(@"Creating sample ticket types...");
            valueProgress.Report(0);

            var sampleTicketTypes = await sampleDataService.GetTicketTypesAsync();

            for (var index = 0; index < sampleTicketTypes.Count; index++)
            {
                var ticketType = sampleTicketTypes[index];

                if (!await SaveTicketTypeAsync(ticketType))
                {
                    throw new InvalidOperationException($"Failed to save ticket type: {ticketType.Name}.");
                }

                valueProgress.Report((index + 1) * 100 / sampleTicketTypes.Count);
            }

            descriptionProgress.Report(string.Empty);
        }

        private async Task InitializeShowTimes(IProgress<int> valueProgress,
            IProgress<string> descriptionProgress)
        {
            descriptionProgress.Report(@"Creating sample show times...");
            valueProgress.Report(0);

            var sampleShowTimes = await sampleDataService.GetShowTimesAsync();

            for (var index = 0; index < sampleShowTimes.Count; index++)
            {
                var showTime = sampleShowTimes[index];

                if (!await SaveShowTimeAsync(showTime))
                {
                    throw new InvalidOperationException(
                        $"Failed to save show time:\n" +
                        $"  Movie: {showTime.Movie.Name}\n" +
                        $"  Auditorium: {showTime.Auditorium.Name}\n" +
                        $"  Time: {showTime.StartTime:t}");
                }

                valueProgress.Report((index + 1) * 100 / sampleShowTimes.Count);
            }

            descriptionProgress.Report(string.Empty);
        }

        private async Task InitializeTickets(IProgress<int> valueProgress,
            IProgress<string> descriptionProgress)
        {
            descriptionProgress.Report(@"Creating sample tickets...");
            valueProgress.Report(0);

            var sampleTickets = await sampleDataService.GetTicketsAsync();
            
            for (var index = 0; index < sampleTickets.Count; index++)
            {
                var ticket = sampleTickets[index];

                if (!await SaveTicketAsync(ticket))
                {
                    throw new InvalidOperationException(
                        "Failed to save ticket:\n" +
                        $"  Movie: {ticket.ShowTime.Movie.Name}\n" +
                        $"  Auditorium: {ticket.ShowTime.Auditorium.Name}\n" +
                        $"  Time: {ticket.ShowTime.StartTime:t}");
                }

                valueProgress.Report((index + 1) * 100 / sampleTickets.Count);
            }

            descriptionProgress.Report(string.Empty);
        }

        private async Task InitializeLogEntries(IProgress<int> valueProgress,
            IProgress<string> descriptionProgress)
        {
            descriptionProgress.Report(@"Creating sample log entries...");
            valueProgress.Report(0);

            var sampleLogEntries = await sampleDataService.GetLogEntriesAsync();

            for (var index = 0; index < sampleLogEntries.Count; index++)
            {
                var logEntry = sampleLogEntries[index];

                if (!await SaveLogEntryAsync(logEntry))
                {
                    throw new InvalidOperationException(
                        "Failed to save log entry:\n" +
                        $"  User: {logEntry.User.UserName}\n" +
                        $"  Description: {logEntry.Message}");
                }

                valueProgress.Report((index + 1) * 100 / sampleLogEntries.Count);
            }

            descriptionProgress.Report(string.Empty);
        }

        private async Task InitializeDailyActivities(IProgress<int> valueProgress,
            IProgress<string> descriptionProgress)
        {
            descriptionProgress.Report(@"Creating sample daily records...");
            valueProgress.Report(0);

            var sampleDailyRecords = await sampleDataService.GetDailyRecordsAsync();

            for (var index = 0; index < sampleDailyRecords.Count; index++)
            {
                var dailyRecord = sampleDailyRecords[index];

                if (!await SaveDailyRecordAsync(dailyRecord))
                {
                    throw new InvalidOperationException(
                        "Failed to save daily record for " +
                        $"{dailyRecord.Created:d}");
                }

                valueProgress.Report((index + 1) * 100 / sampleDailyRecords.Count);
            }

            descriptionProgress.Report(string.Empty);
        }
    }
}

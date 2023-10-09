using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Data;
using C868.Capstone.Services.Data;

namespace C868.Capstone.Core.Reports.Builders
{
    public class MovieSalesReportBuilder : ReportBuilder
    {
        private readonly List<Movie> movies;
        private readonly bool showByWeek;

        private List<TicketType> allTicketTypes;
        private Dictionary<int, List<ShowTime>> mappedShowTimes;
        private Dictionary<int, List<Ticket>> mappedTickets;

        private int lineWidth;
        private int nameWidth; 
        private int ticketTypeWidth;

        public MovieSalesReportBuilder(string title, IEnumerable<Movie> movies, DateTime startDate,
            DateTime endDate, IDataService dataService, bool showByWeek = true)
            : base(title, startDate, endDate, dataService)
        {
            this.movies = movies.ToList();
            this.showByWeek = showByWeek;
        }

        public override async Task<string> BuildReport(IProgress<int> valueProgress,
            IProgress<string> descriptionProgress)
        {
            valueProgress.Report(0);
            
            await InitializeData(valueProgress, descriptionProgress);
            InitializeLayout();

            AppendTitle(lineWidth);
            Builder.Append(Environment.NewLine);

            // Append movies, reporting progress
            for (var index = 0; index < movies.Count; index++)
            {
                var currentMovie = movies[index];

                descriptionProgress.Report($"Processing {currentMovie.Name}...");
                AppendMovie(currentMovie);
                valueProgress.Report((index + 1) * 100 / movies.Count);
            }

            descriptionProgress.Report(string.Empty);

            return Builder.ToString();
        }

        private async Task InitializeData(IProgress<int> valueProgress,
            IProgress<string> descriptionProgress)
        {
            descriptionProgress.Report(@"Loading ticket types...");
            allTicketTypes = (await DataService.GetTicketTypesAsync())
                .OrderBy(ticketType => ticketType.Name)
                .ToList();

            descriptionProgress.Report(@"Loading show times...");
            mappedShowTimes = await GetMappedShowTimes();

            descriptionProgress.Report(@"Loading tickets...");
            mappedTickets = await GetMappedTickets();
        }

        private async Task<Dictionary<int, List<ShowTime>>> GetMappedShowTimes()
        {
            var allShowTimes = await DataService.GetShowTimesAsync(StartDate, EndDate);

            return movies
                .DefaultIfEmpty()
                .ToDictionary(
                    movie => movie.MovieId,
                    movie => allShowTimes
                        .Where(showTime => IsShowTimeIncludedForMovie(showTime) &&
                                           showTime.MovieId == movie.MovieId)
                        .OrderBy(showTime => showTime.StartTime.Date)
                        .ToList());
        }

        private async Task<Dictionary<int, List<Ticket>>> GetMappedTickets()
        {
            var allTickets = await DataService.GetTicketsAsync(StartDate, EndDate);

            return movies
                .DefaultIfEmpty()
                .ToDictionary(
                    movie => movie.MovieId,
                    movie => allTickets
                        .Where(ticket => ticket.ShowTime.MovieId == movie.MovieId)
                        .ToList());
        }

        private List<(DateTime Start, DateTime End)> GetMovieWeeks(List<ShowTime> showTimes)
        {
            var result = new List<(DateTime, DateTime)>();

            var distinctDates = showTimes
                .Select(showTime => showTime.StartTime.Date)
                .Distinct()
                .OrderBy(date => date)
                .ToList();

            // Add the full weeks
            var fullWeekCount = distinctDates.Count / 7;
            for (var index = 0; index < fullWeekCount; index++)
            {
                var startIndex = index * 7;
                var endIndex = index * 7 + 6;

                result.Add((distinctDates[startIndex], distinctDates[endIndex]));
            }

            // Add the remainder days, if any
            if (fullWeekCount * 7 != distinctDates.Count)
            {
                result.Add((distinctDates[fullWeekCount * 7], distinctDates.Last()));
            }

            return result;
        }

        private void InitializeLayout()
        {
            nameWidth = GetNameMaxWidth();
            ticketTypeWidth = GetTicketTypeMaxWidth();

            lineWidth = (ticketTypeWidth * allTicketTypes.Count) + nameWidth;
        }

        private int GetNameMaxWidth()
        {
            // Find the maximum required width for the movie name
            var defaultWeekWidth = "  Week ## (##/##/#### - ##/##/####)".Length;
            var width = movies
                .DefaultIfEmpty()
                .Aggregate(defaultWeekWidth, (max, current) =>
                    max > current.Name.Length ? max : (current.Name + "  ").Length);

            // Pad the width to the next multiple of 5
            do
            {
                width++;
            } while (width % 5 != 0);

            return width;
        }

        private int GetTicketTypeMaxWidth()
        {
            // Find the maximum required width for the ticket type name
            var width = allTicketTypes
                .DefaultIfEmpty()
                .Aggregate(0, (max, current) =>
                    max > current.Name.Length ? max : (current.Name + "  ").Length);

            // Pad the width to the next multiple of 5
            do
            {
                width++;
            } while (nameWidth % 5 != 0);

            return width;
        }

        private void AppendMovie(Movie movie)
        {
            var showTimes = mappedShowTimes[movie.MovieId];
            var tickets = mappedTickets[movie.MovieId];
            
            AppendHeader(movie);

            if (showByWeek)
            {
                var weeks = GetMovieWeeks(showTimes);
                foreach (var week in weeks)
                {
                    var weekNumber = weeks.IndexOf(week) + 1;
                    AppendWeek(tickets, weekNumber, week.Start, week.End);
                }

                AppendBoxMiddle(lineWidth);
                AppendTotals(movie, @"GRAND TOTALS:");
            }
            else
            {
                var description = StartDate == EndDate
                    ? $"  {StartDate:d}"
                    : $"  {StartDate:d} - {EndDate:d}";
                AppendTotals(movie, description);
            }

            AppendBoxBottom(lineWidth);
            Builder.Append(Environment.NewLine);
        }

        private void AppendHeader(Movie movie)
        {
            AppendBoxTop(lineWidth);

            // Ticket Type names
            Builder.Append(@"│");
            Builder.Append(new string(' ', nameWidth));

            foreach (var ticketType in allTicketTypes)
            {
                Builder.Append($"{ticketType.Name} ".PadLeft(ticketTypeWidth));
            }

            Builder.Append(@"│");
            Builder.Append(Environment.NewLine);

            // Movie name and Ticket Type prices
            Builder.Append(@"│");
            Builder.Append($" {movie.Name}".PadRight(nameWidth));

            foreach (var ticketType in allTicketTypes)
            {
                Builder.Append($"{ticketType.Price:C} ".PadLeft(ticketTypeWidth));
            }

            Builder.Append(@"│");
            Builder.Append(Environment.NewLine);

            AppendBoxMiddle(lineWidth);
        }

        private void AppendWeek(List<Ticket> tickets, int weekNumber,
            DateTime weekStart, DateTime weekEnd)
        {
            var weeklyTickets = tickets
                .Where(ticket => IsTicketIncludedForWeek(ticket, weekStart, weekEnd))
                .ToList();

            var weekName = $"  Week {weekNumber} ({weekStart:d} - {weekEnd:d})";
            Builder.Append(@"│");
            Builder.Append(weekName.PadRight(nameWidth));

            AppendWeekCounts(weeklyTickets);
            AppendWeekTotals(weeklyTickets);
        }

        private void AppendWeekCounts(List<Ticket> weeklyTickets)
        {
            foreach (var ticketType in allTicketTypes)
            {
                var count = weeklyTickets
                    .Where(ticket => ticket.TicketTypeId == ticketType.TicketTypeId)
                    .Sum(ticket => ticket.Count);

                Builder.Append($"{count} ".PadLeft(ticketTypeWidth));
            }
            Builder.Append(@"│");
            Builder.Append(Environment.NewLine);
        }

        private void AppendWeekTotals(List<Ticket> weeklyTickets)
        {
            Builder.Append(@"│");
            Builder.Append(new string(' ', nameWidth));

            foreach (var ticketType in allTicketTypes)
            {
                var total = weeklyTickets
                    .Where(ticket => ticket.TicketTypeId == ticketType.TicketTypeId)
                    .Sum(ticket => ticket.Count * ticketType.Price);

                Builder.Append($"{total:C} ".PadLeft(ticketTypeWidth));
            }
            Builder.Append(@"│");
            Builder.Append(Environment.NewLine);
        }

        private void AppendTotals(Movie movie, string description)
        {
            AppendMovieCounts(movie, description);
            AppendMovieTotals(movie);
        }

        private void AppendMovieCounts(Movie movie, string description)
        {
            var tickets = mappedTickets[movie.MovieId];

            Builder.Append(@"│");
            Builder.Append($" {description}".PadRight(nameWidth));

            foreach (var ticketType in allTicketTypes)
            {
                var count = tickets
                    .Where(ticket => ticket.TicketTypeId == ticketType.TicketTypeId)
                    .Sum(ticket => ticket.Count);

                Builder.Append($"{count} ".PadLeft(ticketTypeWidth));
            }

            Builder.Append(@"│");
            Builder.Append(Environment.NewLine);
        }

        private void AppendMovieTotals(Movie movie)
        {
            var tickets = mappedTickets[movie.MovieId];

            Builder.Append(@"│");
            Builder.Append(new string(' ', nameWidth));

            foreach (var ticketType in allTicketTypes)
            {
                var total = tickets
                    .Where(ticket => ticket.TicketTypeId == ticketType.TicketTypeId)
                    .Sum(ticket => ticket.Count * ticketType.Price);

                Builder.Append($"{total:C} ".PadLeft(ticketTypeWidth));
            }
            Builder.Append(@"│");
            Builder.Append(Environment.NewLine);
        }

        private bool IsShowTimeIncludedForMovie(ShowTime showTime)
        {
            return showTime.StartTime.Date >= StartDate.Date &&
                   showTime.StartTime.Date <= EndDate.Date;
        }

        private bool IsTicketIncludedForWeek(Ticket ticket, DateTime weekStart, DateTime weekEnd)
        {
            var ticketDate = ticket.ShowTime.StartTime.Date;

            return ticketDate >= weekStart.Date &&
                   ticketDate <= weekEnd.Date;
        }
    }
}
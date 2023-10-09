using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using C868.Capstone.Core;
using C868.Capstone.Core.Models.Activities;
using C868.Capstone.Core.Models.Data;

namespace C868.Capstone.Services.Data.Sample
{
    public partial class SampleDataService : IDataService
    {
        public SampleDataService()
        {
        }

        public async Task InitializeTables(IProgress<int> valueProgress,
            IProgress<string> descriptionProgress)
        {
            await Task.Run(() =>
            {
                users = new List<User>();
                auditoriums = new List<Auditorium>();
                movies = new List<Movie>();
                ticketTypes = new List<TicketType>();
                showTimes = new List<ShowTime>();
                tickets = new List<Ticket>();
                dailyRecords = new List<DailyRecord>();
                logEntries = new List<LogEntry>();
            });

            if (!await HasUsersAsync())
            {
                await InitializeUsers();
            }
        }

        public async Task InitializeData(IProgress<int> valueProgress, IProgress<string> descriptionProgress)
        {
            if (auditoriums.Count == 0)
            {
                await InitializeAuditoriums();
            }
            
            if (movies.Count == 0)
            {
                await InitializeMovies();
            }
            
            if (ticketTypes.Count == 0)
            {
                await InitializeTicketTypes();
            }
            
            if (showTimes.Count == 0)
            {
                await InitializeShowTimes();
            }
            
            if (tickets.Count == 0)
            {
                await InitializeTickets();
            }

            if (logEntries.Count == 0)
            {
                await InitializeLogEntries();
            }
            
            if (dailyRecords.Count == 0)
            {
                await InitializeDailyRecords();
            }
        }

        private async Task InitializeUsers()
        {
            // Administrator
            await SaveUserAsync(
                new User
                {
                    UserName = "admin",
                    Password = "admin",
                    UserType = UserType.Administrator
                });

            // User
            await SaveUserAsync(
                new User
                {
                    UserName = "user",
                    Password = "user",
                    UserType = UserType.User
                });
        }

        private async Task InitializeAuditoriums()
        {
            await SaveAuditoriumAsync(
                new Auditorium
                {
                    Name = "North A",
                    Capacity = 300
                });

            await SaveAuditoriumAsync(
                new Auditorium
                {
                    Name = "North B",
                    Capacity = 300
                });

            await SaveAuditoriumAsync(
                new Auditorium
                {
                    Name = "South A",
                    Capacity = 270
                });

            await SaveAuditoriumAsync(
                new Auditorium
                {
                    Name = "South B",
                    Capacity = 270
                });

            await SaveAuditoriumAsync(
                new Auditorium
                {
                    Name = "East A",
                    Capacity = 180
                });

            await SaveAuditoriumAsync(
                new Auditorium
                {
                    Name = "East B",
                    Capacity = 180
                });

            await SaveAuditoriumAsync(
                new Auditorium
                {
                    Name = "West A",
                    Capacity = 150
                });

            await SaveAuditoriumAsync(
                new Auditorium
                {
                    Name = "West B",
                    Capacity = 150
                });
        }

        private async Task InitializeMovies()
        {
            // Rated G movie
            await SaveMovieAsync(
                new Movie
                {
                    Name = "Monsters, Inc.",
                    Rating = Rating.G,
                    RunTime = 92,
                    Cast = "Billy Crystal, John Goodman, Steve Buscemi, James Coburn",
                    Description =
                        "In order to power the city, monsters have to scare children so that " +
                        "they scream. However, the children are toxic to the monsters, and " +
                        "after a child gets through, two monsters realize things may not be " +
                        "what they think."
                });

            // Rated PG movie
            await SaveMovieAsync(
                new Movie
                {
                    Name = "The Princess Bride",
                    Rating = Rating.PG,
                    RunTime = 98,
                    Cast = "Cary Elwes, Mandy Patinkin, Robin Wright",
                    Description =
                        "A bedridden boy's grandfather reads him the story of a " +
                        "farmboy-turned-pirate who encounters numerous obstacles, enemies and " +
                        "allies in his quest to be reunited with his true love."
                });

            // Rated PG13 movie
            await SaveMovieAsync(
                new Movie
                {
                    Name = "Tremors",
                    Rating = Rating.PG13,
                    RunTime = 96,
                    Cast = "Kevin Bacon, Fred Ward, Michael Gross",
                    Description =
                        "Natives of a small isolated town defend themselves against strange " +
                        "underground creatures which are killing them one by one."
                });

            // Rated R movie
            await SaveMovieAsync(
                new Movie
                {
                    Name = "High Fidelity",
                    Rating = Rating.R,
                    RunTime = 113,
                    Cast = "John Cusack, Jack Black, Iben Hjejle, Todd Louiso",
                    Description =
                        "Rob, a record store owner and compulsive list maker, recounts his " +
                        "top five breakups, including the one in progress."
                });

            // Rated NC17 movie
            await SaveMovieAsync(
                new Movie
                {
                    Name = "The Evil Dead",
                    Rating = Rating.NC17,
                    RunTime = 85,
                    Cast = "Bruce Campbell, Ellen Sandweiss, Richard DeManincor",
                    Description =
                        "Five friends travel to a cabin in the woods, where they unknowingly " +
                        "release flesh-possessing demons."
                });

            // Other movies
            await SaveMovieAsync(
                new Movie
                {
                    Name = "Terminator 2: Judgment Day",
                    Rating = Rating.R,
                    RunTime = 137,
                    Cast = "Arnold Schwarzenegger, Linda Hamilton, Edward Furlong",
                    Description =
                        "A cyborg, identical to the one who failed to kill Sarah Connor, must " +
                        "now protect her ten year old son John from an even more advanced and " +
                        "powerful cyborg."
                });

            await SaveMovieAsync(
                new Movie
                {
                    Name = "Indiana Jones and the Last Crusade",
                    Rating = Rating.PG13,
                    RunTime = 127,
                    Cast = "Harrison Ford, Sean Connery",
                    Description =
                        "In 1938, after his father goes missing while pursuing the Holy Grail, " +
                        "Indiana Jones finds himself up against the Nazis again to stop them " +
                        "from obtaining its powers."
                });

            await SaveMovieAsync(
                new Movie
                {
                    Name = "Clue",
                    Rating = Rating.PG,
                    RunTime = 94,
                    Cast = 
                        "Tim Curry, Christopher Lloyd, Madeline Kahn, Lesley Ann Warren, " +
                        "Michael McKean, Eileen Brennan, Martin Mull",
                    Description =
                        "Six guests are anonymously invited to a strange mansion for dinner, " +
                        "but after their host is killed, they must cooperate with the staff to " +
                        "identify the murderer as the bodies pile up."
                });
        }

        private async Task InitializeTicketTypes()
        {
            var defaultDateTime = new DateTime();

            await SaveTicketTypeAsync(
                new TicketType
                {
                    Name = "Child",
                    Price = 6.50d,
                    Ratings = Rating.G | Rating.PG | Rating.PG13,
                    TicketTimeType = TicketTimeType.After,
                    StartTime = new DateTime(
                        defaultDateTime.Year,
                        defaultDateTime.Month,
                        defaultDateTime.Day,
                        AppSettings.Default.Times.MatineeEndHour,
                        AppSettings.Default.Times.MatineeEndMinute,
                        AppSettings.Default.Times.MatineeEndSecond),
                    EndTime = new DateTime(
                        defaultDateTime.Year,
                        defaultDateTime.Month,
                        defaultDateTime.Day,
                        AppSettings.Default.Times.MatineeEndHour,
                        AppSettings.Default.Times.MatineeEndMinute,
                        AppSettings.Default.Times.MatineeEndSecond)
                });

            await SaveTicketTypeAsync(
                new TicketType
                {
                    Name = "Child Matinee",
                    Price = 5.00d,
                    Ratings = Rating.G | Rating.PG | Rating.PG13,
                    TicketTimeType = TicketTimeType.During,
                    StartTime = new DateTime(
                        defaultDateTime.Year,
                        defaultDateTime.Month,
                        defaultDateTime.Day,
                        AppSettings.Default.Times.MatineeStartHour,
                        AppSettings.Default.Times.MatineeStartMinute,
                        AppSettings.Default.Times.MatineeStartSecond),
                    EndTime = new DateTime(
                        defaultDateTime.Year,
                        defaultDateTime.Month,
                        defaultDateTime.Day,
                        AppSettings.Default.Times.MatineeEndHour,
                        AppSettings.Default.Times.MatineeEndMinute,
                        AppSettings.Default.Times.MatineeEndSecond)
                });

            await SaveTicketTypeAsync(
                new TicketType
                {
                    Name = "Child Early Bird",
                    Price = 3.00d,
                    Ratings = Rating.G | Rating.PG | Rating.PG13,
                    TicketTimeType = TicketTimeType.Before,
                    StartTime = new DateTime(
                        defaultDateTime.Year,
                        defaultDateTime.Month,
                        defaultDateTime.Day,
                        AppSettings.Default.Times.MatineeStartHour,
                        AppSettings.Default.Times.MatineeStartMinute,
                        AppSettings.Default.Times.MatineeStartSecond),
                    EndTime = new DateTime(
                        defaultDateTime.Year,
                        defaultDateTime.Month,
                        defaultDateTime.Day,
                        AppSettings.Default.Times.MatineeStartHour,
                        AppSettings.Default.Times.MatineeStartMinute,
                        AppSettings.Default.Times.MatineeStartSecond)
                });



            await SaveTicketTypeAsync(
                new TicketType
                {
                    Name = "Adult",
                    Price = 8.00d,
                    Ratings = Rating.G | Rating.PG | Rating.PG13 | Rating.R | Rating.NC17,
                    TicketTimeType = TicketTimeType.After,
                    StartTime = new DateTime(
                        defaultDateTime.Year,
                        defaultDateTime.Month,
                        defaultDateTime.Day,
                        AppSettings.Default.Times.MatineeEndHour,
                        AppSettings.Default.Times.MatineeEndMinute,
                        AppSettings.Default.Times.MatineeEndSecond),
                    EndTime = new DateTime(
                        defaultDateTime.Year,
                        defaultDateTime.Month,
                        defaultDateTime.Day,
                        AppSettings.Default.Times.MatineeEndHour,
                        AppSettings.Default.Times.MatineeEndMinute,
                        AppSettings.Default.Times.MatineeEndSecond)
                });

            await SaveTicketTypeAsync(
                new TicketType
                {
                    Name = "Adult Matinee",
                    Price = 5.50d,
                    Ratings = Rating.G | Rating.PG | Rating.PG13 | Rating.R | Rating.NC17,
                    TicketTimeType = TicketTimeType.During,
                    StartTime = new DateTime(
                        defaultDateTime.Year,
                        defaultDateTime.Month,
                        defaultDateTime.Day,
                        AppSettings.Default.Times.MatineeStartHour,
                        AppSettings.Default.Times.MatineeStartMinute,
                        AppSettings.Default.Times.MatineeStartSecond),
                    EndTime = new DateTime(
                        defaultDateTime.Year,
                        defaultDateTime.Month,
                        defaultDateTime.Day,
                        AppSettings.Default.Times.MatineeEndHour,
                        AppSettings.Default.Times.MatineeEndMinute,
                        AppSettings.Default.Times.MatineeEndSecond)
                });

            await SaveTicketTypeAsync(
                new TicketType
                {
                    Name = "Adult Early Bird",
                    Price = 4.00d,
                    Ratings = Rating.G | Rating.PG | Rating.PG13 | Rating.R | Rating.NC17,
                    TicketTimeType = TicketTimeType.Before,
                    StartTime = new DateTime(
                        defaultDateTime.Year,
                        defaultDateTime.Month,
                        defaultDateTime.Day,
                        AppSettings.Default.Times.MatineeStartHour,
                        AppSettings.Default.Times.MatineeStartMinute,
                        AppSettings.Default.Times.MatineeStartSecond),
                    EndTime = new DateTime(
                        defaultDateTime.Year,
                        defaultDateTime.Month,
                        defaultDateTime.Day,
                        AppSettings.Default.Times.MatineeStartHour,
                        AppSettings.Default.Times.MatineeStartMinute,
                        AppSettings.Default.Times.MatineeStartSecond)
                });
        }

        private async Task InitializeShowTimes()
        {
            var timesByDay = new Dictionary<DayOfWeek, List<DateTime>>
            {
                {
                    DayOfWeek.Friday, new List<DateTime>
                    {
                        DateTime.Today.AddHours(14),                    // 2 PM
                        DateTime.Today.AddHours(16).AddMinutes(30),     // 4:30 PM
                        DateTime.Today.AddHours(19),                    // 7 PM
                        DateTime.Today.AddHours(21).AddMinutes(30)      // 9:30 PM
                    }
                },
                {
                    DayOfWeek.Saturday, new List<DateTime>
                    {
                        DateTime.Today.AddHours(9),                     // 9 AM
                        DateTime.Today.AddHours(11).AddMinutes(30),     // 11:30 AM
                        DateTime.Today.AddHours(14),                    // 2 PM
                        DateTime.Today.AddHours(16).AddMinutes(30),     // 4:30 PM
                        DateTime.Today.AddHours(19),                    // 7 PM
                        DateTime.Today.AddHours(21).AddMinutes(30)      // 9:30 PM
                    }
                },
                {
                    DayOfWeek.Sunday, new List<DateTime>
                    {
                        DateTime.Today.AddHours(9),                     // 9 AM
                        DateTime.Today.AddHours(11).AddMinutes(30),     // 11:30 AM
                        DateTime.Today.AddHours(14),                    // 2 PM
                        DateTime.Today.AddHours(16).AddMinutes(30),     // 4:30 PM
                        DateTime.Today.AddHours(19),                    // 7 PM
                        DateTime.Today.AddHours(21).AddMinutes(30)      // 9:30 PM
                    }
                },
                {
                    DayOfWeek.Monday, new List<DateTime>
                    {
                        DateTime.Today.AddHours(19),                    // 7 PM
                        DateTime.Today.AddHours(21).AddMinutes(30)      // 9:30 PM
                    }
                },
                {
                    DayOfWeek.Tuesday, new List<DateTime>
                    {
                        DateTime.Today.AddHours(19),                    // 7 PM
                        DateTime.Today.AddHours(21).AddMinutes(30)      // 9:30 PM
                    }
                },
                {
                    DayOfWeek.Wednesday, new List<DateTime>
                    {
                        DateTime.Today.AddHours(16).AddMinutes(30),     // 4:30 PM
                        DateTime.Today.AddHours(19),                    // 7 PM
                        DateTime.Today.AddHours(21).AddMinutes(30)      // 9:30 PM
                    }
                },
                {
                    DayOfWeek.Thursday, new List<DateTime>
                    {
                        DateTime.Today.AddHours(19),                    // 7 PM
                        DateTime.Today.AddHours(21).AddMinutes(30)      // 9:30 PM
                    }
                }
            };

            var availableAuditoriums = await GetAuditoriumsAsync();
            var availableMovies = await GetMoviesAsync();
            var maxCount = Math.Min(availableAuditoriums.Count, availableMovies.Count);

            // Add show times until all auditoriums or all movies are used,
            // whichever comes first
            for (var index = 0; index < maxCount; index++)
            {
                var minuteOffset = index * 10;
                var auditorium = availableAuditoriums[index];
                var movie = availableMovies[index];

                var previousFriday = GetPreviousFriday();
                var nextThursday = GetNextThursday();
                
                // Add show times for each day between last Friday and next Thursday
                for (var currentDate = previousFriday;
                     currentDate <= nextThursday;
                     currentDate = currentDate.AddDays(1))
                {
                    var baseTimes = timesByDay[currentDate.DayOfWeek];

                    // Add show times for each base time plus offset
                    foreach (var currentTime in baseTimes.Select(
                                 baseTime => baseTime.AddMinutes(minuteOffset)))
                    {
                        await SaveShowTimeAsync(
                            new ShowTime
                            {
                                MovieId = movie.MovieId,
                                Movie = movie,
                                AuditoriumId = auditorium.AuditoriumId,
                                Auditorium = auditorium,
                                StartTime = new DateTime(
                                    currentDate.Year,
                                    currentDate.Month,
                                    currentDate.Day,
                                    currentTime.Hour,
                                    currentTime.Minute,
                                    currentTime.Second)
                            });
                    }
                }
            }
        }

        private async Task InitializeTickets()
        {
            var random = new Random();
            var allShowTimes = await GetShowTimesAsync();
            var allTicketTypes = await GetTicketTypesAsync();
            
            foreach (var showTime in allShowTimes)
            {
                if (showTime.StartTime.Date >= DateTime.Today)
                {
                    continue;
                }

                var availableTicketTypes = allTicketTypes
                    .Where(ticketType => ticketType.IsAvailableForShowTime(showTime))
                    .ToList();
                
                foreach (var ticketType in availableTicketTypes)
                {
                    await SaveCashTicket(showTime, ticketType);
                    await SaveCreditTicket(showTime, ticketType);
                    await SaveCheckTicket(showTime, ticketType);
                }
            }
        }

        private async Task SaveCashTicket(ShowTime showTime, TicketType ticketType)
        {
            var minuteOffset = -1 * new Random().Next(30);
            
            var ticket = new Ticket
            {
                Count = 1,
                Payment = PaymentType.Cash,
                TransactionDate = showTime.StartTime.AddMinutes(minuteOffset),
                Created = DateTime.Now,
                TicketTypeId = ticketType.TicketTypeId,
                TicketType = ticketType,
                ShowTimeId = showTime.ShowTimeId,
                ShowTime = showTime
            };
            await SaveTicketAsync(ticket);

        }

        private async Task SaveCreditTicket(ShowTime showTime, TicketType ticketType)
        {
            var minuteOffset = -1 * new Random().Next(30);

            var ticket = new Ticket
            {
                Count = 1,
                Payment = PaymentType.Credit,
                TransactionDate = showTime.StartTime.AddMinutes(minuteOffset),
                Created = DateTime.Now,
                TicketTypeId = ticketType.TicketTypeId,
                TicketType = ticketType,
                ShowTimeId = showTime.ShowTimeId,
                ShowTime = showTime
            };
            await SaveTicketAsync(ticket);
        }

        private async Task SaveCheckTicket(ShowTime showTime, TicketType ticketType)
        {
            var minuteOffset = -1 * new Random().Next(30);

            var ticket = new Ticket
            {
                Count = 1,
                Payment = PaymentType.Check,
                TransactionDate = showTime.StartTime.AddMinutes(minuteOffset),
                Created = DateTime.Now,
                TicketTypeId = ticketType.TicketTypeId,
                TicketType = ticketType,
                ShowTimeId = showTime.ShowTimeId,
                ShowTime = showTime
            };
            await SaveTicketAsync(ticket);
        }

        private async Task InitializeLogEntries()
        {
            await AddMovieCreationLogEntries();
            await AddOpenCloseDayLogEntries();
            await AddAdditionalSampleLogEntries();
        }

        private async Task InitializeDailyRecords()
        {
            var startDate = (await GetShowTimesAsync())
                .Select(showTime => showTime.StartTime.Date)
                .DefaultIfEmpty(DateTime.Today.Date)
                .Min();

            for (var currentDate = startDate;
                 currentDate < DateTime.Today;
                 currentDate = currentDate.AddDays(1))
            {
                var dailyTickets = await GetTicketsAsync(currentDate);

                var dailyRecord = new DailyRecord
                {
                    OpenDate = currentDate.AddHours(8),
                    CloseDate = currentDate.AddHours(23),
                    IsOpen = false,
                    CashExpected = dailyTickets
                        .Where(ticket => ticket.Payment == PaymentType.Cash)
                        .Sum(ticket => ticket.Count * ticket.TicketType.Price),
                    CreditExpected = dailyTickets
                        .Where(ticket => ticket.Payment == PaymentType.Credit)
                        .Sum(ticket => ticket.Count * ticket.TicketType.Price),
                    CheckExpected = dailyTickets
                        .Where(ticket => ticket.Payment == PaymentType.Check)
                        .Sum(ticket => ticket.Count * ticket.TicketType.Price),
                    Created = DateTime.Now
                };

                dailyRecord.CashActual = dailyRecord.CashExpected - 5d;
                dailyRecord.CreditActual = dailyRecord.CreditExpected;
                dailyRecord.CheckActual = dailyRecord.CheckExpected + 5d;

                await SaveDailyRecordAsync(dailyRecord);
            }
        }

        private async Task AddMovieCreationLogEntries()
        {
            var random = new Random();

            var adminUser = users
                .FirstOrDefault(user => user.UserType == UserType.Administrator);

            if (adminUser is null)
            {
                return;
            }

            foreach (var movie in movies)
            {
                var createdDate = (await GetShowTimesAsync(movie))
                    .Select(showTime => showTime.StartTime)
                    .DefaultIfEmpty(DateTime.Today)
                    .Min()
                    .AddDays(-1);

                var logEntry = new LogEntry
                {
                    Type = LogMessageType.Info,
                    Message = $"*SAMPLE* Saved movie: {movie.Name}",
                    Created = new DateTime(
                        createdDate.Year,
                        createdDate.Month,
                        createdDate.Day,
                        random.Next(24),
                        random.Next(60),
                        random.Next(60)),
                    UserId = adminUser.UserId,
                    User = adminUser,
                };

                await SaveLogEntryAsync(logEntry);
            }
        }

        private async Task AddOpenCloseDayLogEntries()
        {
            var random = new Random();
            var normalUser = users
                .FirstOrDefault(user => user.UserType == UserType.User);

            if (normalUser is null)
            {
                return;
            }

            var startDate = (await GetShowTimesAsync())
                .Select(showTime => showTime.StartTime)
                .DefaultIfEmpty(DateTime.Today)
                .Min();


            for (var currentDate = startDate;
                 currentDate < DateTime.Today;
                 currentDate = currentDate.AddDays(1))
            {
                var openLogEntry = new LogEntry
                {
                    Type = LogMessageType.Info,
                    Message = "*SAMPLE* Opened the day",
                    Created = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day,
                        8, random.Next(60), random.Next(60)),
                    UserId = normalUser.UserId,
                    User = normalUser
                };

                await SaveLogEntryAsync(openLogEntry);

                var closeLogEntry = new LogEntry
                {
                    Type = LogMessageType.Info,
                    Message = "*SAMPLE* Closed the day",
                    Created = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day,
                        23, random.Next(60), random.Next(60)),
                    UserId = normalUser.UserId,
                    User = normalUser
                };

                await SaveLogEntryAsync(closeLogEntry);
            }
        }

        private async Task AddAdditionalSampleLogEntries()
        {
            var random = new Random();

            var normalUser = users
                .FirstOrDefault(user => user.UserType == UserType.User);

            if (normalUser is null)
            {
                return;
            }

            var adminUser = users
                .FirstOrDefault(user => user.UserType == UserType.Administrator);

            if (adminUser is null)
            {
                return;
            }

            var startDate = (await GetShowTimesAsync())
                .Select(showTime => showTime.StartTime)
                .DefaultIfEmpty(DateTime.Today)
                .Min();


            // Create five log entries of each type, with random
            // hour/minute/day timestamps for each day that has shows
            for (var currentDate = startDate;
                 currentDate < DateTime.Today;
                 currentDate = currentDate.AddDays(1))
            {
                for (var count = 1; count <= 5; count++)
                {
                    var infoLogEntry = new LogEntry
                    {
                        Type = LogMessageType.Info,
                        Message = $"*SAMPLE* Info Message #{count}",
                        Created = new DateTime(
                            currentDate.Year,
                            currentDate.Month,
                            currentDate.Day,
                            random.Next(24),
                            random.Next(60),
                            random.Next(60)),
                        UserId = normalUser.UserId,
                        User = normalUser
                    };

                    await SaveLogEntryAsync(infoLogEntry);

                    var warningLogEntry = new LogEntry
                    {
                        Type = LogMessageType.Warning,
                        Message = $"*SAMPLE* Warning Message #{count}",
                        Created = new DateTime(
                            currentDate.Year,
                            currentDate.Month,
                            currentDate.Day,
                            random.Next(24),
                            random.Next(60),
                            random.Next(60)),
                        UserId = adminUser.UserId,
                        User = adminUser
                    };

                    await SaveLogEntryAsync(warningLogEntry);

                    var errorLogEntry = new LogEntry
                    {
                        Type = LogMessageType.Error,
                        Message = $"*SAMPLE* Error Message #{count}",
                        Created = new DateTime(
                            currentDate.Year,
                            currentDate.Month,
                            currentDate.Day,
                            random.Next(24),
                            random.Next(60),
                            random.Next(60)),
                        UserId = normalUser.UserId,
                        User = normalUser
                    };

                    await SaveLogEntryAsync(errorLogEntry);
                }
            }
        }

        private DateTime GetPreviousFriday()
        {
            var result = DateTime.Today;
            
            do
            {
                result = result.AddDays(-1);
            } while (result.DayOfWeek != DayOfWeek.Friday);

            return result;
        }

        private DateTime GetNextThursday()
        {
            var result = DateTime.Today;

            do
            {
                result = result.AddDays(1);
            } while (result.DayOfWeek != DayOfWeek.Thursday);

            return result;
        }
    }
}

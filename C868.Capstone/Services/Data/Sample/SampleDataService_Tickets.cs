using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Data;

namespace C868.Capstone.Services.Data.Sample
{
    public partial class SampleDataService
    {
        private List<Ticket> tickets;

        public async Task<Ticket> GetTicketAsync(int ticketId)
        {
            return await Task.FromResult(
                tickets.FirstOrDefault(
                    ticket => ticket.TicketId == ticketId));
        }

        public async Task<List<Ticket>> GetTicketsAsync()
        {
            return await Task.FromResult(new List<Ticket>(tickets));
        }

        public async Task<List<Ticket>> GetTicketsAsync(DateTime date)
        {
            return await Task.FromResult(
                tickets
                    .Where(ticket => ticket.ShowTime.StartTime.Date == date.Date)
                    .ToList());
        }

        public async Task<List<Ticket>> GetTicketsAsync(DateTime startDate, DateTime endDate)
        {
            return await Task.FromResult(
                tickets
                    .Where(ticket => ticket.ShowTime.StartTime.Date >= startDate &&
                                     ticket.ShowTime.StartTime.Date < endDate)
                    .ToList());
        }

        public async Task<bool> SaveTicketAsync(Ticket ticket)
        {
            return await Task.FromResult(
                ticket.TicketId == 0
                    ? await InsertTicket(ticket)
                    : await UpdateTicket(ticket));
        }

        public async Task<bool> DeleteTicketAsync(Ticket ticket)
        {
            var foundTicket = tickets.FirstOrDefault(
                found => found.TicketId == ticket.TicketId);

            return await Task.FromResult(tickets.Remove(foundTicket));
        }

        public async Task<bool> HasTicketsAsync()
        {
            return await Task.FromResult(tickets.Count > 0);
        }

        private async Task<bool> InsertTicket(Ticket newTicket)
        {
            return await Task.Run(() =>
            {
                var lastTicketId = tickets
                    .Select(t => t.TicketId)
                    .DefaultIfEmpty()
                    .Max();

                newTicket.TicketId = lastTicketId + 1;
                tickets.Add(newTicket);

                return true;
            });
        }

        private async Task<bool> UpdateTicket(Ticket newTicket)
        {
            return await Task.Run(() =>
            {
                var oldTicket = tickets
                    .FirstOrDefault(t => t.TicketId == newTicket.TicketId);

                tickets.Add(newTicket);

                return tickets.Remove(oldTicket);
            });
        }
    }
}

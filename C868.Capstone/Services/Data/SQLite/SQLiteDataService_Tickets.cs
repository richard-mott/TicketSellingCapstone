using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Data;
using SQLiteNetExtensionsAsync.Extensions;

namespace C868.Capstone.Services.Data.SQLite
{
    public partial class SQLiteDataService
    {
        public async Task<Ticket> GetTicketAsync(int ticketId)
        {
            return await dbContext.FindWithChildrenAsync<Ticket>(ticketId, recursive: true);
        }

        public async Task<List<Ticket>> GetTicketsAsync()
        {
            return await dbContext.GetAllWithChildrenAsync<Ticket>(recursive: true);
        }

        public async Task<List<Ticket>> GetTicketsAsync(DateTime date)
        {
            var startDate = date.Date;
            var endDate = date.AddDays(1).Date;

            return await dbContext.GetAllWithChildrenAsync<Ticket>(
                ticket => ticket.TransactionDate >= startDate &&
                          ticket.TransactionDate < endDate,
                recursive: true);
        }

        public async Task<List<Ticket>> GetTicketsAsync(DateTime startDate, DateTime endDate)
        {
            return await dbContext.GetAllWithChildrenAsync<Ticket>(
                ticket => ticket.TransactionDate >= startDate &&
                          ticket.TransactionDate < endDate,
                recursive: true);
        }
        
        public async Task<bool> SaveTicketAsync(Ticket ticket)
        {
            var foundTicket = await GetTicketAsync(ticket.TicketId);

            return foundTicket is null
                ? await dbContext.InsertAsync(ticket) == 1
                : await dbContext.UpdateAsync(ticket) == 1;
        }

        public async Task<bool> DeleteTicketAsync(Ticket ticket)
        {
            return await dbContext.DeleteAsync(ticket) == 1;
        }

        public async Task<bool> HasTicketsAsync()
        {
            return await dbContext.Table<Ticket>().CountAsync() > 0;
        }
    }
}

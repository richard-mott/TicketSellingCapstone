using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Data;

namespace C868.Capstone.Services.Data
{
    public partial interface IDataService
    {
        Task<Ticket> GetTicketAsync(int ticketId);
        Task<List<Ticket>> GetTicketsAsync();
        Task<List<Ticket>> GetTicketsAsync(DateTime date);
        Task<List<Ticket>> GetTicketsAsync(DateTime startDate, DateTime endDate);
        Task<bool> SaveTicketAsync(Ticket ticket);
        Task<bool> DeleteTicketAsync(Ticket ticket);
        Task<bool> HasTicketsAsync();
    }
}

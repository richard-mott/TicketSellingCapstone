using System.Collections.Generic;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Data;

namespace C868.Capstone.Services.Data
{
    public partial interface IDataService
    {
        Task<TicketType> GetTicketTypeAsync(int ticketTypeId);
        Task<List<TicketType>> GetTicketTypesAsync();
        Task<bool> SaveTicketTypeAsync(TicketType ticketType);
        Task<bool> DeleteTicketTypeAsync(TicketType ticketType);
        Task<bool> HasTicketTypesAsync();
    }
}

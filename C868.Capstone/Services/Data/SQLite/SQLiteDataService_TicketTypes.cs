using System.Collections.Generic;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Data;

namespace C868.Capstone.Services.Data.SQLite
{
    public partial class SQLiteDataService
    {
        public async Task<TicketType> GetTicketTypeAsync(int ticketTypeId)
        {
            return await dbContext.FindAsync<TicketType>(ticketTypeId);
        }

        public async Task<List<TicketType>> GetTicketTypesAsync()
        {
            return await dbContext.Table<TicketType>().ToListAsync();
        }

        public async Task<bool> SaveTicketTypeAsync(TicketType ticketType)
        {
            var foundTicketType = await GetTicketTypeAsync(ticketType.TicketTypeId);

            return foundTicketType is null
                ? await dbContext.InsertAsync(ticketType) == 1
                : await dbContext.UpdateAsync(ticketType) == 1;
        }

        public async Task<bool> DeleteTicketTypeAsync(TicketType ticketType)
        {
            return await dbContext.DeleteAsync(ticketType) == 1;
        }

        public async Task<bool> HasTicketTypesAsync()
        {
            return await dbContext.Table<TicketType>().CountAsync() > 0;
        }
    }
}

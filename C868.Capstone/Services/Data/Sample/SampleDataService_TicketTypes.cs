using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Data;

namespace C868.Capstone.Services.Data.Sample
{
    public partial class SampleDataService
    {
        private List<TicketType> ticketTypes;

        public async Task<TicketType> GetTicketTypeAsync(int ticketTypeId)
        {
            return await Task.FromResult(
                ticketTypes.FirstOrDefault(
                    ticketType => ticketType.TicketTypeId == ticketTypeId));
        }

        public async Task<List<TicketType>> GetTicketTypesAsync()
        {
            return await Task.FromResult(new List<TicketType>(ticketTypes));
        }

        public async Task<bool> SaveTicketTypeAsync(TicketType ticketType)
        {
            return await Task.FromResult(
                ticketType.TicketTypeId == 0
                    ? await InsertTicketTypeAsync(ticketType)
                    : await UpdateTicketTypeAsync(ticketType));
        }

        public async Task<bool> DeleteTicketTypeAsync(TicketType ticketType)
        {
            var foundTicketType = ticketTypes.FirstOrDefault(
                found => found.TicketTypeId == ticketType.TicketTypeId);

            return await Task.FromResult(ticketTypes.Remove(foundTicketType));
        }

        public async Task<bool> HasTicketTypesAsync()
        {
            return await Task.FromResult(ticketTypes.Count > 0);
        }

        private async Task<bool> InsertTicketTypeAsync(TicketType newTicketType)
        {
            return await Task.Run(() =>
            {
                var lastTicketTypeIndex = ticketTypes
                    .Select(tt => tt.TicketTypeId)
                    .DefaultIfEmpty()
                    .Max();

                newTicketType.TicketTypeId = lastTicketTypeIndex + 1;
                ticketTypes.Add(newTicketType);

                return true;
            });
        }

        private async Task<bool> UpdateTicketTypeAsync(TicketType newTicketType)
        {
            return await Task.Run(() =>
            {
                var oldTicketType = ticketTypes
                    .FirstOrDefault(tt => tt.TicketTypeId == newTicketType.TicketTypeId);

                ticketTypes.Add(newTicketType);

                return ticketTypes.Remove(oldTicketType);
            });
        }
    }
}

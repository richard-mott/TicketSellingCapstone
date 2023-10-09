using System.Collections.Generic;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Data;

namespace C868.Capstone.Services.Data.SQLite
{
    public partial class SQLiteDataService
    {
        public async Task<Auditorium> GetAuditoriumAsync(int auditoriumId)
        {
            return await dbContext.FindAsync<Auditorium>(auditoriumId);
        }

        public async Task<List<Auditorium>> GetAuditoriumsAsync()
        {
            return await dbContext.Table<Auditorium>().ToListAsync();
        }

        public async Task<bool> SaveAuditoriumAsync(Auditorium auditorium)
        {
            var foundAuditorium = await GetAuditoriumAsync(auditorium.AuditoriumId);

            return foundAuditorium is null
                ? await dbContext.InsertAsync(auditorium) == 1
                : await dbContext.UpdateAsync(auditorium) == 1;
        }

        public async Task<bool> DeleteAuditoriumAsync(Auditorium auditorium)
        {
            return await dbContext.DeleteAsync(auditorium) == 1;
        }

        public async Task<bool> HasAuditoriumsAsync()
        {
            return await dbContext.Table<Auditorium>().CountAsync() > 0;
        }
    }
}

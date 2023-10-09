using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Data;

namespace C868.Capstone.Services.Data.Sample
{
    public partial class SampleDataService
    {
        private List<Auditorium> auditoriums;

        public async Task<Auditorium> GetAuditoriumAsync(int auditoriumId)
        {
            return await Task.FromResult(
                auditoriums.FirstOrDefault(
                    auditorium => auditorium.AuditoriumId == auditoriumId));
        }

        public async Task<List<Auditorium>> GetAuditoriumsAsync()
        {
            return await Task.FromResult(new List<Auditorium>(auditoriums));
        }

        public async Task<bool> SaveAuditoriumAsync(Auditorium auditorium)
        {
            return await Task.FromResult(
                auditorium.AuditoriumId == 0
                    ? await InsertAuditoriumAsync(auditorium)
                    : await UpdateAuditoriumAsync(auditorium));
        }

        public async Task<bool> DeleteAuditoriumAsync(Auditorium auditorium)
        {
            var foundAuditorium = auditoriums.FirstOrDefault(
                found => found.AuditoriumId == auditorium.AuditoriumId);

            return await Task.FromResult(auditoriums.Remove(foundAuditorium));
        }

        public async Task<bool> HasAuditoriumsAsync()
        {
            return await Task.FromResult(auditoriums.Count > 0);
        }

        private async Task<bool> InsertAuditoriumAsync(Auditorium newAuditorium)
        {
            return await Task.Run(() =>
            {
                var lastAuditoriumIndex = auditoriums
                    .Select(a => a.AuditoriumId)
                    .DefaultIfEmpty()
                    .Max();

                newAuditorium.AuditoriumId = lastAuditoriumIndex + 1;
                auditoriums.Add(newAuditorium);

                return true;
            });
        }

        private async Task<bool> UpdateAuditoriumAsync(Auditorium newAuditorium)
        {
            return await Task.Run(() =>
            {
                var oldAuditorium = auditoriums.FirstOrDefault(
                    auditorium => auditorium.AuditoriumId == newAuditorium.AuditoriumId);

                auditoriums.Add(newAuditorium);

                return auditoriums.Remove(oldAuditorium);
            });
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Data;

namespace C868.Capstone.Services.Data
{
    public partial interface IDataService
    {
        Task<Auditorium> GetAuditoriumAsync(int auditoriumId);
        Task<List<Auditorium>> GetAuditoriumsAsync();
        Task<bool> SaveAuditoriumAsync(Auditorium auditorium);
        Task<bool> DeleteAuditoriumAsync(Auditorium auditorium);
        Task<bool> HasAuditoriumsAsync();
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Data;

namespace C868.Capstone.Services.Data
{
    public partial interface IDataService
    {
        Task<User> GetUserAsync(int userId);
        Task<List<User>> GetUsersAsync();
        Task<bool> SaveUserAsync(User user);
        Task<bool> DeleteUserAsync(User user);
        Task<bool> HasUsersAsync();

        Task<User> ValidateUserAsync(string userName, string password);
    }
}

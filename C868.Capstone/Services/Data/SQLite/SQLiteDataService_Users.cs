using System.Collections.Generic;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Data;

namespace C868.Capstone.Services.Data.SQLite
{
    public partial class SQLiteDataService
    {
        public async Task<User> GetUserAsync(int userId)
        {
            return await dbContext.FindAsync<User>(userId);
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await dbContext.Table<User>().ToListAsync();
        }

        public async Task<bool> SaveUserAsync(User user)
        {
            var foundUser = await GetUserAsync(user.UserId);

            return foundUser is null
                ? await dbContext.InsertAsync(user) == 1
                : await dbContext.UpdateAsync(user) == 1;
        }

        public async Task<bool> DeleteUserAsync(User user)
        {
            return await dbContext.DeleteAsync(user) == 1;
        }

        public async Task<bool> HasUsersAsync()
        {
            return await dbContext.Table<User>().CountAsync() > 0;
        }

        public async Task<User> ValidateUserAsync(
            string userName, string password)
        {
            var foundUser = await dbContext.FindAsync<User>(
                user => user.UserName == userName);
            
            return foundUser?.Password == password
                ? foundUser
                : null;
        }
    }
}

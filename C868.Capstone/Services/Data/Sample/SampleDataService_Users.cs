using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Data;

namespace C868.Capstone.Services.Data.Sample
{
    public partial class SampleDataService
    {
        private List<User> users;

        public async Task<User> GetUserAsync(int userId)
        {
            return await Task.FromResult(
                users.FirstOrDefault(
                    user => user.UserId == userId));
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await Task.FromResult(new List<User>(users));
        }

        public async Task<bool> SaveUserAsync(User user)
        {
            return await Task.FromResult(
                user.UserId == 0
                    ? await InsertUserAsync(user)
                    : await UpdateUserAsync(user));
        }

        public async Task<bool> DeleteUserAsync(User user)
        {
            var foundUser = users.FirstOrDefault(
                found => found.UserId == user.UserId);

            return await Task.FromResult(users.Remove(foundUser));
        }

        public async Task<bool> HasUsersAsync()
        {
            return await Task.FromResult(users.Count > 0);
        }

        public async Task<User> ValidateUserAsync(string userName, string password)
        {
            return await Task.FromResult(
                users.FirstOrDefault(
                    user => user.UserName == userName &&
                            user.Password == password));
        }

        private async Task<bool> InsertUserAsync(User newUser)
        {
            return await Task.Run(() =>
            {
                var lastUserIndex = users
                    .Select(u => u.UserId)
                    .DefaultIfEmpty()
                    .Max();

                newUser.UserId = lastUserIndex + 1;
                users.Add(newUser);

                return true;
            });
        }

        private async Task<bool> UpdateUserAsync(User newUser)
        {
            return await Task.Run(() =>
            {
                var oldUser = users
                    .FirstOrDefault(u => u.UserId == newUser.UserId);

                users.Add(newUser);

                return users.Remove(oldUser);
            });
        }
    }
}

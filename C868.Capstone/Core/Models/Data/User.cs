using System;
using SQLite;

namespace C868.Capstone.Core.Models.Data
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int UserId { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public UserType UserType { get; set; }

        public DateTime Created { get; set; }

        public User()
        {
            UserId = 0;

            UserName = string.Empty;
            Password = string.Empty;
            UserType = UserType.User;
            Created = DateTime.Now;
        }

        public User Clone()
        {
            return new User
            {
                UserId = UserId,

                UserName = UserName,
                Password = Password,
                UserType = UserType,
                Created = Created
            };
        }
    }
}

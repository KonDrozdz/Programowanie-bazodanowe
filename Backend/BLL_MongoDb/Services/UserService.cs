using BLL.ServiceInterfaces;
using BLL.DTOModels;
using BLL_MongoDb.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace BLL_MongoDb.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<UserGroup> _groups;

        public UserService(IMongoDatabase db)
        {
            _users = db.GetCollection<User>("Users");
            _groups = db.GetCollection<UserGroup>("UserGroups");
        }

        public bool Login(string login, string passwordHash)
        {
            return _users.Find(u => u.Login == login && u.PasswordHash == passwordHash).Any();
        }

        public void Logout(int userId)
        {
            
        }

        public void AddUser(UserRequestDTO userDto)
        {
            var user = new User
            {
                Login = userDto.Login,
                PasswordHash = userDto.Password,
            };
            _users.InsertOne(user);
        }

        public IEnumerable<UserGroupResponseDTO> GetUserGroups()
        {
            return _groups.Find(_ => true)
                .ToList()
                .Select(g => new UserGroupResponseDTO
                {
                    Id = int.Parse(g.Id),
                    Name = g.Name
                });
        }

        public Task<UserResponseDTO> LoginAsync(string login, string password)
        {
            throw new NotImplementedException();
        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }
    }
}
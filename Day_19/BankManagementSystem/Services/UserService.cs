using System.Collections.Generic;
using System.Threading.Tasks;
using BankManagementSystem.Models;
using BankManagementSystem.Contexts;
using Microsoft.EntityFrameworkCore;
using BankManagementSystem.Interfaces;
using BankManagementSystem.Misc;
using BankManagementSystem.Models.DTOs;

namespace BankManagementSystem.Services
{
    public class UserService : IUserService
    {
        UserMapper userMapper;
        private readonly IRepository<int, User> _userRepository;

        public UserService(IRepository<int, User> userRepository)
        {
            userMapper = new UserMapper();
            _userRepository = userRepository;
        }

        public async Task<User> GetUserById(int userId)
        {
            try
            {
                var user = await _userRepository.Get(userId);
                return user;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<ICollection<User>> GetAllUsers()
        {
            var users = await _userRepository.GetAll();
            return users.ToList();
        }

        public async Task<User> AddUser(UserAddRequestDto userAddRequestDto)
        {
            var user = userMapper.MapUserAddRequestDtoToUser(userAddRequestDto);
            user = await _userRepository.Add(user);
            return user;
        }
    }
}

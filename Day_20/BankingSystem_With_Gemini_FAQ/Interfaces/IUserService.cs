using System;
using BankManagementSystem.Models;
using BankManagementSystem.Models.DTOs;

namespace BankManagementSystem.Interfaces
{
    public interface IUserService
    {
        public Task<User> AddUser(UserAddRequestDto userAddRequestDto);
        public Task<User> GetUserById(int userId);
        public Task<ICollection<User>> GetAllUsers();
    }
}
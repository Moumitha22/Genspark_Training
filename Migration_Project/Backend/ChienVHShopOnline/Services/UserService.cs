using AutoMapper;
using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Models.DTOs;
using ChienVHShopOnline.Repositories;

namespace ChienVHShopOnline.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
        }

        // public async Task<UserResponseDto> CreateUser(UserRequestDto userDto)
        // {
        //     var user = _mapper.Map<User>(userDto);
        //     var created = await _userRepo.Add(user);
        //     return _mapper.Map<UserResponseDto>(created);
        // }

        public async Task<UserResponseDto> CreateUser(User user)
        {
            var created = await _userRepo.Add(user);
            return _mapper.Map<UserResponseDto>(created);
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsers()
        {
            var users = await _userRepo.GetAll();
            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }

        public async Task<UserResponseDto> GetUserById(int id)
        {
            var user = await _userRepo.Get(id);
            if (user == null)
                throw new Exception("User not found");

            return _mapper.Map<UserResponseDto>(user);
        }
        
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            email = email.Trim().ToLowerInvariant();
            return await _userRepo.GetByEmailAsync(email);
        }

        public async Task<UserResponseDto> UpdateUser(int id, UserUpdateDto userDto)
        {
            var existingUser = await _userRepo.Get(id);
            if (existingUser == null)
                throw new Exception("User not found");

            _mapper.Map(userDto, existingUser);
            var updated = await _userRepo.Update(id, existingUser);

            return _mapper.Map<UserResponseDto>(updated);
        }

        public async Task<UserResponseDto> DeleteUser(int id)
        {
            var deleted = await _userRepo.Delete(id);
            return _mapper.Map<UserResponseDto>(deleted);
        }
    }
}


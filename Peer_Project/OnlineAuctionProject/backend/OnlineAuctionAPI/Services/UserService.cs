using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using OnlineAuctionAPI.Contexts;
using OnlineAuctionAPI.Exceptions;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;

    public UserService(
        IUserRepository userRepository,
        IMapper mapper,
        IPasswordService passwordService,
        AuctionContext auctionContext,
        IEmailService emailService
    )
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _mapper = mapper;
        _emailService = emailService;
    }

    public async Task<User> CreateUserAsync(UserRegisterRequestDto userDto)
    {
        var existingUserData = await _userRepository.GetByEmailAsync(userDto.Email);
        if (existingUserData != null)
        {
            throw new AlreadyExistsException("Some user already exists with this email.");
        }
        var user = _mapper.Map<User>(userDto);
        user.Password = _passwordService.HashPassword(user.Password);
        await _userRepository.Add(user);
        return user;
    }

    // public async Task<User> DeleteUserAsync(UserDeleteRequest userDeleteDto)
    // {
    //     var user = await _userRepository.Get(userDeleteDto.UserId);
    //     if (user.StatusId == 2)
    //     {
    //         throw new AlreadyDeletedException("User is already deleted");
    //     }

    //     user.StatusId = 2;
    //     await _userRepository.Update(user.Id, user);
    //     await _auctionContext.DeletedUsers.AddAsync(
    //         new DeletedUsers
    //         {
    //             UserId = userDeleteDto.UserId,
    //             Email = user.Email,
    //             Reason = userDeleteDto.Reason,
    //             DeletedAt = DateTime.UtcNow,
    //         }
    //     );
    //     await _auctionContext.SaveChangesAsync();
    //     return user;
    // }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            throw new NullValueException("Email can't be null or empty please ensure it");
        }
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            throw new NotFoundException(
                $"The given email - {email} is not found, please verify that."
            );
        }
        return user;
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        var users = await _userRepository.GetAll();
        if (users == null || !users.Any())
        {
            throw new NotFoundException("No users found");
        }
        return users;
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.Get(id);
        return user;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            throw new NullValueException("User name can't be null or empty, please ensure it");
        }
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
        {
            throw new NotFoundException($"The user with username {username} is not found");
        }
        return user;
    }

    public async Task<User> UpdateUserInfoAsync(Guid id, UserUpdateRequestDto updateuserdto)
    {
        var user = await _userRepository.Get(id);
        if (user == null)
        {
            throw new NotFoundException(
                $"The user with Id {id} is not found, please ensure the UserId"
            );
        }
        _mapper.Map(updateuserdto, user);
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.Update(id, user);
        return user;
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordRequestDto changePasswordRequestDto)
    {
        var user = await _userRepository.Get(changePasswordRequestDto.UserId);
        if (user == null)
            throw new NotFoundException("User not found");

        if (
            !_passwordService.VerifyPassword(
                user.Password,
                changePasswordRequestDto.CurrentPassword
            )
        )
            throw new InvalidException("Current password is incorrect");

        user.Password = _passwordService.HashPassword(changePasswordRequestDto.NewPassword);
        await _userRepository.Update(changePasswordRequestDto.UserId, user);
        return true;
    }

    public async Task<bool> ForgetPasswordAsync(ForgetPasswordRequestDto forgetPasswordRequestDto)
    {
        if (string.IsNullOrEmpty(forgetPasswordRequestDto.Email))
        {
            throw new NullValueException("Email can't be null or empty, please ensure it");
        }
        var user = await _userRepository.GetByEmailAsync(forgetPasswordRequestDto.Email);
        if (user == null)
        {
            throw new NotFoundException(
                $"The user with email {forgetPasswordRequestDto.Email} is not found, please verify that."
            );
        }
        user.Password = _passwordService.HashPassword(forgetPasswordRequestDto.NewPassword);
        await _userRepository.Update(user.Id, user);
        return true;
    }

    public async Task<User> GetUserWithWalletByUserIdAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdWithVirtualWalletAsync(userId);
        if (user == null)
        {
            throw new NotFoundException($"No user found with the given User Id: {userId}");
        }
        return user;
    }

    public async Task<IEnumerable<User>> SearchUsersAsync(UserSearchDto searchDto)
    {
        var users = await _userRepository.GetAll();
        var searchTerm = searchDto.SearchTerm ?? string.Empty;
        var filteredUsers = users
            .Where(u =>
                u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                || u.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            )
            .ToList();

        if (searchDto.SortBy != null)
        {
            filteredUsers = searchDto.SortBy.ToLower() switch
            {
                "username" => [.. filteredUsers.OrderBy(u => u.Username)],
                "email" => [.. filteredUsers.OrderBy(u => u.Email)],
                "createdat" => [.. filteredUsers.OrderBy(u => u.CreatedAt)],
                "role" => [.. filteredUsers.OrderByDescending(u => u.Role)],
                "updatedat" => [.. filteredUsers.OrderBy(u => u.UpdatedAt)],
                _ => throw new InvalidException("Invalid sort option provided"),
            };
        }

        return filteredUsers;
    }

    public async Task<User> CreateAdminAsync(UserRegisterRequestDto userDto)
    {
        if (userDto == null)
        {
            throw new NullValueException("User data cannot be null");
        }

        var existingUserData = await _userRepository.GetByEmailAsync(userDto.Email);
        if (existingUserData != null)
        {
            throw new AlreadyExistsException("Some user already exists with this email.");
        }

        var adminUser = _mapper.Map<User>(userDto);
        adminUser.Password = _passwordService.HashPassword(adminUser.Password);
        var addAdmin = await _userRepository.Add(adminUser);
        if (addAdmin != null)
        {
            try
            {
                await _emailService.SendEmailAsync(
                    addAdmin.Email,
                    "Welcome to Online Auction",
                    @"You have been successfully registered as an admin.
                Your account is now active and you can log in using your credentials.
                Password format: your username's first 2 letters, @, 123 (e.g., ab@123)"
                );
            }
            catch
            {
                Console.WriteLine("Failed to send email notification to the admin user.");
            }
        }
        return adminUser;
    }
}

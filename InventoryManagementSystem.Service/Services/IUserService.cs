using InventoryManagementSystem.Service.DTOs;

namespace InventoryManagementSystem.Service.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(string id);
        Task<UserDto> CreateUserAsync(UserCreateDto userDto, string createdById);
        Task<UserDto> UpdateUserAsync(string id, UserUpdateDto userDto, string modifiedById);
        Task DeleteUserAsync(string id);
        Task<TokenResponseDto> LoginAsync(UserLoginDto loginDto);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
        Task<bool> UpdateLastLoginAsync(string userId);
    }
}

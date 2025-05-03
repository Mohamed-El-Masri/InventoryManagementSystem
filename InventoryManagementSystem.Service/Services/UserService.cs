using AutoMapper;
using InventoryManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InventoryManagementSystem.Service.DTOs;
using InventoryManagementSystem.Service.Exceptions;

namespace InventoryManagementSystem.Service.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(
            UserManager<User> userManager,
            IMapper mapper,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = _userManager.Users.Where(u => !u.UserName.Equals("admin"));
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto?> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return null;

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> CreateUserAsync(UserCreateDto userDto, string createdById)
        {
            var existingUserByEmail = await _userManager.FindByEmailAsync(userDto.Email);
            if (existingUserByEmail != null)
                throw new InvalidOperationException("Email is already in use");

            var existingUserByName = await _userManager.FindByNameAsync(userDto.UserName);
            if (existingUserByName != null)
                throw new InvalidOperationException("Username is already taken");

            var user = new User
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Role = userDto.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdById
            };

            var result = await _userManager.CreateAsync(user, userDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create user: {errors}");
            }

            await _userManager.AddToRoleAsync(user, userDto.Role.ToString());

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdateUserAsync(string id, UserUpdateDto userDto, string modifiedById)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new NotFoundException($"User with ID {id} not found");

            // Check if email is changed and if it's already in use
            if (user.Email != userDto.Email)
            {
                var existingUserByEmail = await _userManager.FindByEmailAsync(userDto.Email);
                if (existingUserByEmail != null && existingUserByEmail.Id != id)
                    throw new InvalidOperationException("Email is already in use");
                
                user.Email = userDto.Email;
            }

            // Update user properties
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.IsActive = userDto.IsActive;
            
            // Update role if changed
            if (user.Role != userDto.Role)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, userDto.Role.ToString());
                user.Role = userDto.Role;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to update user: {errors}");
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new NotFoundException($"User with ID {id} not found");

            // Instead of deleting, we can deactivate the user
            user.IsActive = false;
            await _userManager.UpdateAsync(user);
            
            // Or hard delete if required:
            // var result = await _userManager.DeleteAsync(user);
            // if (!result.Succeeded)
            // {
            //     var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            //     throw new InvalidOperationException($"Failed to delete user: {errors}");
            // }
        }

        public async Task<TokenResponseDto> LoginAsync(UserLoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null)
                throw new NotFoundException("Invalid username or password");

            if (!user.IsActive)
                throw new InvalidOperationException("User account is deactivated");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
                throw new NotFoundException("Invalid username or password");

            var roles = await _userManager.GetRolesAsync(user);
            
            var token = GenerateJwtToken(user, roles.FirstOrDefault() ?? "Employee");
            
            // Update last login date
            await UpdateLastLoginAsync(user.Id);

            return token;
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
        {
            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
                throw new InvalidOperationException("New password and confirm password do not match");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException($"User with ID {userId} not found");

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to change password: {errors}");
            }

            return true;
        }

        public async Task<bool> UpdateLastLoginAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException($"User with ID {userId} not found");

            user.LastLoginDate = DateTime.UtcNow;
            var result = await _userManager.UpdateAsync(user);
            
            return result.Succeeded;
        }

        private TokenResponseDto GenerateJwtToken(User user, string role)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expirationMinutes = int.Parse(jwtSettings["ExpirationInMinutes"] ?? "60");
            
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(expirationMinutes);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var encodedToken = tokenHandler.WriteToken(token);

            return new TokenResponseDto
            {
                Token = encodedToken,
                Expiration = expiration,
                UserId = user.Id,
                UserName = user.UserName,
                Role = role
            };
        }
    }
}

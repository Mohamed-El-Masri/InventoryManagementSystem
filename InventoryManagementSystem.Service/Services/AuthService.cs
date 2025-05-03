using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Service.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InventoryManagementSystem.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<User> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid username or password");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("This account is disabled");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
                throw new UnauthorizedAccessException("Invalid username or password");

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Employee";

            var token = await GenerateJwtTokenAsync(user, role);

            // Update last login date
            user.LastLoginDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Username = user.UserName,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Role = role
            };
        }

        public async Task<bool> RegisterUserAsync(RegisterDto registerDto)
        {
            // Check if user already exists
            if (await _userManager.FindByNameAsync(registerDto.Username) != null)
                throw new InvalidOperationException("Username is already taken");

            if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
                throw new InvalidOperationException("Email is already taken");

            // Create new user
            var user = new User
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Role = Domain.Enums.UserRole.Employee, // Default role
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                throw new InvalidOperationException($"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            // Add to Employee role by default
            await _userManager.AddToRoleAsync(user, "Employee");

            return true;
        }

        private async Task<string> GenerateJwtTokenAsync(User user, string role)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT secret key is not configured");
            var issuer = jwtSettings["Issuer"] ?? "InventorySystem";
            var audience = jwtSettings["Audience"] ?? "InventorySystemUsers";
            var expirationMinutes = int.Parse(jwtSettings["ExpirationInMinutes"] ?? "60");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

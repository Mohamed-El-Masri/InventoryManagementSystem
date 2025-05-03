using FluentAssertions;
using InventoryManagementSystem.API.Controllers;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Service.DTOs;
using InventoryManagementSystem.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace InventoryManagementSystem.Tests.Integration
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockLogger = new Mock<ILogger<AuthController>>();
            _authController = new AuthController(_mockAuthService.Object, _mockLogger.Object);
            
            // Setup controller context
            _authController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnOkWithToken()
        {
            // Arrange
            var loginDto = new LoginDto { Username = "testuser", Password = "Password123!" };
            var authResponse = new AuthResponseDto
            {
                Token = "test-jwt-token",
                UserId = "user123",
                Username = "testuser",
                Role = "Employee"
            };

            _mockAuthService.Setup(service => service.LoginAsync(loginDto)).ReturnsAsync(authResponse);

            // Act
            var result = await _authController.Login(loginDto);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(authResponse);
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDto { Username = "testuser", Password = "WrongPassword!" };
            
            _mockAuthService.Setup(service => service.LoginAsync(loginDto))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid username or password"));

            // Act
            var result = await _authController.Login(loginDto);

            // Assert
            var unauthorizedResult = result.Result as UnauthorizedObjectResult;
            unauthorizedResult.Should().NotBeNull();
            unauthorizedResult!.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        public async Task Register_WithValidData_ShouldReturnOk()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "Password123!",
                FirstName = "New",
                LastName = "User"
            };

            _mockAuthService.Setup(service => service.RegisterUserAsync(registerDto)).ReturnsAsync(true);

            // Act
            var result = await _authController.Register(registerDto);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task Register_WithExistingUsername_ShouldReturnBadRequest()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "existinguser",
                Email = "newuser@example.com",
                Password = "Password123!",
                FirstName = "New",
                LastName = "User"
            };

            _mockAuthService.Setup(service => service.RegisterUserAsync(registerDto))
                .ThrowsAsync(new InvalidOperationException("Username is already taken"));

            // Act
            var result = await _authController.Register(registerDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }
    }
}

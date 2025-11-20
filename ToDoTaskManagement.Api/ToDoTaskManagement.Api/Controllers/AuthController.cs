using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Claims;
using System.Text;
using ToDoTaskManagement.Api.Services;
using ToDoTaskManagement.Api.Settings;
using ToDoTaskManagement.Application.DTOs.Auth;
using ToDoTaskManagement.Application.Interfaces;
using ToDoTaskManagement.Domain.Entities;
using ToDoTaskManagement.Domain.Interfaces;

namespace ToDoTaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenService _tokenService;
        private readonly JwtSettings _jwtSettings;
        private readonly IEmailSender _emailSender;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenService tokenService,
            IOptions<JwtSettings> jwtOptions,
            IEmailSender emailSender,
            IRefreshTokenService refreshTokenService,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _jwtSettings = jwtOptions.Value;
            _emailSender = emailSender;
            _refreshTokenService = refreshTokenService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid register attempt: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            if (await _userManager.FindByEmailAsync(dto.Email) != null)
            {
                _logger.LogWarning("Registration failed: email already exists {Email}", dto.Email);
                ModelState.AddModelError("Email", "Email is already taken.");
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser { UserName = dto.UserName, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Registration failed for {User}. Errors: {Errors}", dto.UserName, result.Errors);
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            // Generate email confirmation token and send email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmUrl = $"{Request.Scheme}://{Request.Host}/api/auth/confirm-email?userId={user.Id}&token={encodedToken}";
            await _emailSender.SendEmailAsync(user.Email, "Confirm your account",
                $"Please confirm your account by clicking <a href=\"{confirmUrl}\">here</a>.");

            _logger.LogInformation("User registered: {UserId}. Confirmation email sent to {Email}", user.Id, user.Email);

            return Accepted(new { Message = "Registration successful. Please check email to confirm account." });
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return BadRequest("UserId and token are required.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            var decodedBytes = WebEncoders.Base64UrlDecode(token);
            var decodedToken = Encoding.UTF8.GetString(decodedBytes);
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Email confirmation failed for {UserId}", userId);
                return BadRequest("Email confirmation failed.");
            }

            _logger.LogInformation("Email confirmed for {UserId}", userId);
            return Ok("Email confirmed.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid login attempt data: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            ApplicationUser? user = dto.UserNameOrEmail.Contains("@")
                ? await _userManager.FindByEmailAsync(dto.UserNameOrEmail)
                : await _userManager.FindByNameAsync(dto.UserNameOrEmail);

            if (user == null)
            {
                _logger.LogWarning("Invalid login attempt for identifier {Identifier}", dto.UserNameOrEmail);
                // Do not reveal whether user exists
                return Unauthorized("Invalid credentials.");
            }

            // Check email confirmed
            if (!_userManager.Options.SignIn.RequireConfirmedEmail ? false : !await _userManager.IsEmailConfirmedAsync(user))
            {
                _logger.LogWarning("Login attempt before email confirmation for user {UserId}", user.Id);
                return Unauthorized("Email not confirmed.");
            }

            // Use SignInManager to ensure lockoutOnFailure is respected
            var signInResult = await _signInManager.PasswordSignInAsync(user, dto.Password, isPersistent: false, lockoutOnFailure: true);

            if (signInResult.IsLockedOut)
            {
                _logger.LogWarning("User account locked out: {UserId}", user.Id);
                return Unauthorized("Account locked. Try again later.");
            }

            if (!signInResult.Succeeded)
            {
                _logger.LogWarning("Invalid credentials for user {UserId}", user.Id);
                return Unauthorized("Invalid credentials.");
            }

            // Create access token
            var accessToken = _tokenService.GenerateToken(user.Id, new[] {
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            });

            // Create refresh token (persist)
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var refreshToken = await _refreshTokenService.CreateAsync(user.Id, ip ?? "unknown");

            var response = new TokenResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresMinutes)
            };

            _logger.LogInformation("User {UserId} logged in successfully", user.Id);

            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var rt = await _refreshTokenService.GetByTokenAsync(dto.RefreshToken);
            if (rt == null)
            {
                _logger.LogWarning("Refresh token not found {Token}", dto.RefreshToken);
                return Unauthorized("Invalid refresh token.");
            }

            if (rt.ExpiresAt <= DateTime.UtcNow)
            {
                _logger.LogWarning("Refresh token expired id={Id}", rt.Id);
                return Unauthorized("Refresh token expired.");
            }

            if (rt.Revoked)
            {
                _logger.LogWarning("Refresh token revoked id={Id}", rt.Id);
                return Unauthorized("Refresh token revoked.");
            }

            // rotate: revoke old token and issue a new one
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var newRt = await _refreshTokenService.CreateAsync(rt.UserId, ip ?? "unknown");
            await _refreshTokenService.RevokeAsync(rt, replacedBy: newRt.Token);

            // Issue new access token
            var accessToken = _tokenService.GenerateToken(rt.UserId, new[] {
                new Claim(ClaimTypes.NameIdentifier, rt.UserId)
            });

            var response = new TokenResponseDto
            {
                Token = accessToken,
                RefreshToken = newRt.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresMinutes)
            };

            _logger.LogInformation("Refreshed token for user {UserId}", rt.UserId);
            return Ok(response);
        }

        [HttpPost("revoke")]
        [Authorize]
        public async Task<IActionResult> Revoke([FromBody] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken)) return BadRequest("Refresh token required.");

            var rt = await _refreshTokenService.GetByTokenAsync(refreshToken);
            if (rt == null) return NotFound();

            // Only owner can revoke
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            if (rt.UserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to revoke token that belongs to {Owner}", userId, rt.UserId);
                return Forbid();
            }

            await _refreshTokenService.RevokeAsync(rt);
            _logger.LogInformation("User {UserId} revoked refresh token id={Id}", userId, rt.Id);
            return Ok();
        }
    }
}
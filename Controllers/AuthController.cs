using Microsoft.AspNetCore.Mvc;
using backend.Services;
using backend.DTOs.Auth;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ICaptchaService _captchaService;
        private readonly IRsaKeyService _rsaKeyService;

        public AuthController(AuthService authService, ICaptchaService captchaService, IRsaKeyService rsaKeyService)
        {
            _authService = authService;
            _captchaService = captchaService;
            _rsaKeyService = rsaKeyService;
        }

        // GET /api/auth/public-key
        [HttpGet("public-key")]
        public IActionResult GetPublicKey()
        {
            var publicKey = _rsaKeyService.GetPublicKey();
            return Ok(new { publicKey });
        }

        // POST /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            Console.WriteLine("\n--- BACKEND AUTH FLOW START ---");
            Console.WriteLine($"1. AuthController.Login: Attempt for User='{request.Username}'");
            Console.WriteLine($"2. AuthController.Login: Captcha Data: ID='{request.CaptchaId}', Input='{request.CaptchaInput}'");

            // Validate captcha
            if (string.IsNullOrEmpty(request.CaptchaId) || string.IsNullOrEmpty(request.CaptchaInput))
            {
                Console.WriteLine("3. AuthController.Login: ABORTED - Captcha fields missing");
                return BadRequest(new { message = "Captcha is required" });
            }

            var captchaValid = _captchaService.ValidateCaptcha(request.CaptchaId, request.CaptchaInput);
            Console.WriteLine($"3. AuthController.Login: Captcha Validation result: {captchaValid}");
            
            if (!captchaValid)
            {
                return BadRequest(new { message = "Invalid or expired captcha" });
            }

            Console.WriteLine("4. AuthController.Login: Calling AuthService.LoginAsync...");
            var (success, token, user, message) = await _authService.LoginAsync(request.Username, request.Password);
            Console.WriteLine($"11. AuthController.Login: AuthService result for {request.Username}: Success={success}, Message='{message}'");

            if (!success)
            {
                return BadRequest(new { message });
            }

            Console.WriteLine("12. AuthController.Login: SUCCESS. Returning Token and User info.");
            return Ok(new { token, user });
        }

        // POST /api/auth/signup
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            var (success, message) = await _authService.SignupAsync(request.Username, request.Password, request.Name, request.Phone);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }

        // POST /api/auth/guest-login
        [HttpPost("guest-login")]
        public IActionResult GuestLogin([FromBody] GuestLoginRequest request)
        {
            var (success, otp, message) = _authService.GuestLogin(request.Name, request.Phone);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message, otp }); // Remove otp from response in production
        }

        // POST /api/auth/verify-otp
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var (success, token, user, message) = await _authService.VerifyOtpAsync(request.Phone, request.Otp);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { token, user });
        }

        // POST /api/auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var (success, message) = await _authService.ResetPasswordAsync(request.Phone, request.Otp, request.NewPassword);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }

        // POST /api/auth/change-password
        [HttpPost("change-password")]
        [Microsoft.AspNetCore.Authorization.Authorize] // Require user to be logged in
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            // Extract the user ID from the JWT Token claims
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid token or user ID not found." });
            }

            var (success, message) = await _authService.ChangePasswordAsync(userId, request.OldPassword, request.NewPassword);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }


        // POST /api/auth/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // In a production environment with token blacklist, you would:
            // 1. Extract token from Authorization header
            // 2. Add it to a blacklist/revoked tokens store
            // 3. Set expiry time to match token's original expiry
            
            // For now, return success - client-side handles token removal
            return Ok(new { message = "Logged out successfully" });
        }

        [HttpPost("forgot-password/request-otp")]
        public async Task<IActionResult> ForgotPasswordRequestOtp([FromBody] ForgotPasswordRequestOtp request)
        {
            var (success, message) = await _authService.ForgotPasswordRequestOtpAsync(request.Username);
            return Ok(new { message }); // Always returns Ok to prevent user enumeration
        }

        [HttpPost("forgot-password/verify-otp")]
        public async Task<IActionResult> ForgotPasswordVerifyOtp([FromBody] ForgotPasswordVerifyOtp request)
        {
            var (success, token, message) = await _authService.ForgotPasswordVerifyOtpAsync(request.Username, request.Otp);
            if (!success)
            {
                return BadRequest(new { message });
            }
            return Ok(new { token, message });
        }

        [HttpPost("forgot-password/reset")]
        public async Task<IActionResult> ForgotPasswordReset([FromBody] ForgotPasswordReset request)
        {
            var (success, message) = await _authService.ForgotPasswordResetAsync(request.ResetToken, request.NewPassword);
            if (!success)
            {
                return BadRequest(new { message });
            }
            return Ok(new { message });
        }
    }

    // Request DTOs
    public record LoginRequest(string Username, string Password, string CaptchaId, string CaptchaInput);
    public record SignupRequest(string Username, string Password, string Name, string Phone);
    public record GuestLoginRequest(string Name, string Phone);
    public record VerifyOtpRequest(string Phone, string Otp);
    public record ResetPasswordRequest(string Phone, string Otp, string NewPassword);
    public record ChangePasswordRequest(string OldPassword, string NewPassword);
}
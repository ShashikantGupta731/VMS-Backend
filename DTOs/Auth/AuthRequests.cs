namespace backend.DTOs.Auth
{
    public record LoginRequest(string Username, string Password, string CaptchaId, string CaptchaInput);
    public record SignupRequest(string Username, string Password, string Name, string Phone);
    public record GuestLoginRequest(string Name, string Phone);
    public record VerifyOtpRequest(string Phone, string Otp);
    public record ResetPasswordRequest(string Phone, string Otp, string NewPassword);
}

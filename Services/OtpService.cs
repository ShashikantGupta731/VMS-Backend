namespace backend.Services
{
    public class OtpService
    {
        // In-memory storage for OTPs (key: phone, value: (otp, expiry, name))
        private static readonly Dictionary<string, (string Otp, DateTime Expiry, string Name)> _otpStore = new();

        // Generate a 6-digit OTP
        public string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        // Store OTP with phone number, name, and 5-minute expiry
        public void StoreOtp(string phone, string otp, string name = "")
        {
            var expiry = DateTime.UtcNow.AddMinutes(5); // 5 minutes expiry
            _otpStore[phone] = (otp, expiry, name);
        }

        // Verify OTP and return name
        public (bool IsValid, string Name) VerifyOtp(string phone, string otp)
        {
            if (!_otpStore.ContainsKey(phone))
                return (false, "");

            var storedOtp = _otpStore[phone];

            // Check if OTP expired
            if (DateTime.UtcNow > storedOtp.Expiry)
            {
                _otpStore.Remove(phone);
                return (false, "");
            }

            // Check if OTP matches
            if (storedOtp.Otp == otp)
            {
                var name = storedOtp.Name;
                _otpStore.Remove(phone); // Remove after successful verification
                return (true, name);
            }

            return (false, "");
        }

        // Clean up expired OTPs (optional - can be called periodically)
        public void CleanupExpiredOtps()
        {
            var expiredKeys = _otpStore
                .Where(kvp => DateTime.UtcNow > kvp.Value.Expiry)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                _otpStore.Remove(key);
            }
        }
    }
}
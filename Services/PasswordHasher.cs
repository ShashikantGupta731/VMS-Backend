using BCrypt.Net;

namespace backend.Services
{
    public static class PasswordHasher
    {
        // Hash a password
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Verify a password against a hash
        public static bool VerifyPassword(string password, string hash)
        {
            Console.WriteLine($"8. PasswordHasher.VerifyPassword: Entry. InputPwdLength={password?.Length ?? 0}, HashFromDb='{(string.IsNullOrEmpty(hash) ? "NULL" : hash.Substring(0, Math.Min(10, hash.Length)))}...'");
            try
            {
                if (string.IsNullOrEmpty(hash) || !hash.StartsWith("$2"))
                {
                    Console.WriteLine($"9. PasswordHasher.VerifyPassword: ABORTED. Hash format is INVALID (starts with: {(string.IsNullOrEmpty(hash) ? "N/A" : hash.Substring(0, Math.Min(5, hash.Length)))})");
                    return false;
                }
                
                Console.WriteLine("9. PasswordHasher.VerifyPassword: Calling BCrypt.Verify...");
                bool result = BCrypt.Net.BCrypt.Verify(password, hash);
                Console.WriteLine($"10. PasswordHasher.VerifyPassword: BCrypt Result={result}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"9. PasswordHasher.VerifyPassword: EXCEPTION during BCrypt call: {ex.Message}");
                return false;
            }
        }
    }
}
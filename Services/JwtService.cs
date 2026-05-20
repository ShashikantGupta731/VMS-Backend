using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using backend.Models.Core;

namespace backend.Services
{
    public class JwtService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtService(IConfiguration configuration)
        {
            _secretKey = configuration["Jwt:SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
            _issuer = configuration["Jwt:Issuer"] ?? "VMS";
            _audience = configuration["Jwt:Audience"] ?? "VMSUsers";
        }

        // Generate JWT token for a user
        public string GenerateToken(User user, List<string> roles)
        {
            Console.WriteLine($"[JWT SERVICE] Generating Token for User='{user.Username}', ID={user.UserId}");
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? ""),
                new Claim(ClaimTypes.GivenName, user.Name ?? ""),
                new Claim("phone", user.PhoneNo ?? ""),
                new Claim("isGuest", user.IsGuest.ToString()),
                
                // --- NEW JURISDICTION CLAIMS ---
                new Claim("departmentId", user.DeptId?.ToString() ?? ""),
                new Claim("districtId", user.DistrictId?.ToString() ?? ""),
                new Claim("ddoCode", user.DDOCode ?? ""),
                new Claim("DDOCode", user.DDOCode ?? "")
            };


            // Add roles as claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            
            Console.WriteLine($"[JWT SERVICE] Added {claims.Count} claims to token payload.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7), // 7 days as per requirement
                signingCredentials: credentials
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            Console.WriteLine("[JWT SERVICE] Token generation COMPLETE.");
            return tokenString;
        }

        // Validate JWT token and return claims principal
        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _issuer,
                    ValidAudience = _audience,
                    IssuerSigningKey = key,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        // Get user ID from token
        public int? GetUserIdFromToken(string token)
        {
            var principal = ValidateToken(token);
            if (principal == null) return null;

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return null;

            if (int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            return null;
        }
    }
}
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace backend.Services
{
    public interface ICaptchaService
    {
        (string CaptchaId, byte[] ImageBytes) GenerateCaptcha();
        bool ValidateCaptcha(string captchaId, string userInput);
    }

    public class CaptchaService : ICaptchaService
    {
        private static readonly Dictionary<string, CaptchaData> _captchaStore = new();
        private static readonly Random _random = new();
        private static readonly object _lock = new();
        private const int CaptchaExpiryMinutes = 5;
        private const int CaptchaLength = 6;

        // Generate a new captcha
        public (string CaptchaId, byte[] ImageBytes) GenerateCaptcha()
        {
            var captchaId = Guid.NewGuid().ToString();
            var captchaText = GenerateRandomText(CaptchaLength);
            var imageBytes = GenerateCaptchaImage(captchaText);

            // Store captcha with expiry
            lock (_lock)
            {
                _captchaStore[captchaId] = new CaptchaData
                {
                    Text = captchaText,
                    Expiry = DateTime.UtcNow.AddMinutes(CaptchaExpiryMinutes)
                };

                // Clean up expired captchas
                CleanupExpiredCaptchas();
            }

            return (captchaId, imageBytes);
        }

        // Validate captcha
        public bool ValidateCaptcha(string captchaId, string userInput)
        {
            lock (_lock)
            {
                if (!_captchaStore.ContainsKey(captchaId))
                {
                    Console.WriteLine($"[CAPTCHA] FAIL: ID '{captchaId}' not found in store.");
                    return false;
                }

                var captchaData = _captchaStore[captchaId];
                Console.WriteLine($"[CAPTCHA] Checking: Input='{userInput}', StoreValue='{captchaData.Text}', Expiry={captchaData.Expiry}");

                // Check if expired
                if (DateTime.UtcNow > captchaData.Expiry)
                {
                    Console.WriteLine("[CAPTCHA] FAIL: Captcha expired.");
                    _captchaStore.Remove(captchaId);
                    return false;
                }

                // Validate input (case-insensitive)
                var isValid = captchaData.Text.Equals(userInput, StringComparison.OrdinalIgnoreCase);
                Console.WriteLine($"[CAPTCHA] Match result: {isValid}");

                // Remove captcha after validation (one-time use)
                _captchaStore.Remove(captchaId);

                return isValid;
            }
        }

        // Generate random alphanumeric text
        private string GenerateRandomText(int length)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Removed similar looking chars (I, 1, O, 0)
            var result = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[_random.Next(chars.Length)]);
            }

            return result.ToString();
        }

        // Generate captcha image
        private byte[] GenerateCaptchaImage(string text)
        {
            using var bitmap = new Bitmap(100, 38);
            using var graphics = Graphics.FromImage(bitmap);

            // Background
            graphics.Clear(Color.FromArgb(240, 240, 240));

            // Add noise lines
            for (int i = 0; i < 10; i++)
            {
                var pen = new Pen(GetRandomColor(), 1);
                var x1 = _random.Next(0, bitmap.Width);
                var y1 = _random.Next(0, bitmap.Height);
                var x2 = _random.Next(0, bitmap.Width);
                var y2 = _random.Next(0, bitmap.Height);
                graphics.DrawLine(pen, x1, y1, x2, y2);
            }

            // Add noise dots
            for (int i = 0; i < 100; i++)
            {
                var dotBrush = new SolidBrush(GetRandomColor());
                var dotX = _random.Next(0, bitmap.Width);
                var dotY = _random.Next(0, bitmap.Height);
                graphics.FillRectangle(dotBrush, dotX, dotY, 2, 2);
            }

            // Draw text with slight distortion
            var font = new Font("Arial", 16, FontStyle.Bold);
            var brush = new SolidBrush(Color.FromArgb(50, 50, 50));
            
            // Draw each character with slight rotation
            var x = 5;
            foreach (char c in text)
            {
                var angle = _random.Next(-15, 15);
                graphics.TranslateTransform(x + 8, 12);
                graphics.RotateTransform(angle);
                graphics.DrawString(c.ToString(), font, brush, 0, 0);
                graphics.ResetTransform();
                x += 14;
            }

            // Convert to byte array
            using var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }

        // Get random color
        private Color GetRandomColor()
        {
            return Color.FromArgb(
                _random.Next(100, 200),
                _random.Next(100, 200),
                _random.Next(100, 200)
            );
        }

        // Clean up expired captchas
        private void CleanupExpiredCaptchas()
        {
            var expired = _captchaStore
                .Where(kvp => DateTime.UtcNow > kvp.Value.Expiry)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expired)
            {
                _captchaStore.Remove(key);
            }
        }

        // Captcha data holder
        private class CaptchaData
        {
            public string Text { get; set; } = string.Empty;
            public DateTime Expiry { get; set; }
        }
    }
}

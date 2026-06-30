using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace backend.Services
{
    public class SmsService : ISmsService
    {
        private readonly ILogger<SmsService> _logger;

        public SmsService(ILogger<SmsService> logger)
        {
            _logger = logger;
        }

        public Task<bool> SendOtpAsync(string phoneNumber, string otpText)
        {
            // In a real production scenario, you would integrate with the IFMS SMS Gateway here
            // using the HttpSmsResponseMessage and OtpPayload classes.
            // For now, we simulate SMS delivery by logging it to the console/logger.
            
            _logger.LogInformation($"[SMS MOCK] Sending OTP '{otpText}' to {phoneNumber}...");
            Console.WriteLine($"\n[SMS MOCK] OTP for {phoneNumber} is: {otpText}\n");
            
            return Task.FromResult(true);
        }
    }
}

using System.Threading.Tasks;

namespace backend.Services
{
    public interface ISmsService
    {
        Task<bool> SendOtpAsync(string phoneNumber, string otpText);
    }
}

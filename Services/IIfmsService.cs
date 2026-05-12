using backend.DTOs.Auth;

namespace backend.Services
{
    public interface IIfmsService
    {
        Task<(bool Success, IfmsLoginResponseData? Data, string? Message)> LoginViaIfmsAsync(string username, string password);
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
        string GenerateChecksum(string json);
    }
}

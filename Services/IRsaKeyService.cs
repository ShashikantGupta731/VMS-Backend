using System.Security.Cryptography;

namespace backend.Services
{
    public interface IRsaKeyService
    {
        string GetPublicKey();
        string DecryptAesKey(string encryptedAesKeyBase64);
    }
}

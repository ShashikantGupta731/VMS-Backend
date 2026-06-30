using System;
using System.Security.Cryptography;

namespace backend.Services
{
    public class RsaKeyService : IRsaKeyService
    {
        private readonly RSA _rsa;
        private readonly string _publicKeyBase64;

        public RsaKeyService()
        {
            // Initialize RSA with 2048-bit key
            _rsa = RSA.Create(2048);
            
            // Export public key to base64 using standard SPKI format
            var publicKeyBytes = _rsa.ExportSubjectPublicKeyInfo();
            _publicKeyBase64 = Convert.ToBase64String(publicKeyBytes);
            Console.WriteLine("[RsaKeyService] RSA Key Pair generated. Public Key is ready.");
        }

        public string GetPublicKey()
        {
            return _publicKeyBase64;
        }

        public string DecryptAesKey(string encryptedAesKeyBase64)
        {
            try
            {
                var encryptedBytes = Convert.FromBase64String(encryptedAesKeyBase64);
                // We use OaepSHA256 padding for broad compatibility with frontend crypto libraries
                var decryptedBytes = _rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
                // The frontend encrypted the string value of the AES key, so we read it as UTF8 string
                return System.Text.Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RsaKeyService] Decryption failed: {ex.Message}");
                return $"ERROR: {ex.Message}";
            }
        }
    }
}

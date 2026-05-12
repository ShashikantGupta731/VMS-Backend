using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using backend.DTOs.Auth;
using Microsoft.Extensions.Configuration;

namespace backend.Services
{
    public class IfmsService : IIfmsService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly string _checksumKey;
        private readonly string _encryptionKey;
        private readonly string _encryptionIv;
        private readonly string _ifmsBaseUrl;

        public IfmsService(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
            
            // These will need to be added to appsettings.json
            _checksumKey = _config["Auth2Keys:VMS:JWTKeys:ChecksumKey"] ?? "DEFAULT_CHECKSUM_KEY";
            _encryptionKey = _config["Auth2Keys:VMS:JWTKeys:SecretKey"] ?? "DEFAULT_SECRET_KEY";
            _encryptionIv = _config["Auth2Keys:VMS:JWTKeys:SecretIV"] ?? "DEFAULT_SECRET_IV";
            _ifmsBaseUrl = _config["IFMSUrl:Vms"] ?? "https://ifms.punjab.gov.in/api/";
        }

        public async Task<(bool Success, IfmsLoginResponseData? Data, string? Message)> LoginViaIfmsAsync(string username, string password)
        {
            try
            {
                // 1. Create the inner payload
                var loginPayload = new IfmsLoginRequest { username = username, password = password };
                string innerJson = JsonSerializer.Serialize(loginPayload);

                // 2. Wrap with Checksum (Legacy used HMACSHA512)
                var encryptedPayload = new IfmsEncryptedPayload
                {
                    data = loginPayload,
                    checksum = GenerateChecksum(innerJson)
                };
                string outerJson = JsonSerializer.Serialize(encryptedPayload);

                // 3. Encrypt and wrap for Transport
                var requestWrapper = new IfmsRequestWrapper
                {
                    encData = Encrypt(outerJson),
                    client_id = _config["Auth2Keys:VMS:Header:ClientId"] ?? "",
                    client_secret = _config["Auth2Keys:VMS:Header:ClientSecret"] ?? "",
                    ipaddress = _config["Auth2Keys:VMS:Header:IPAllow"] ?? "",
                    integration_src = _config["Auth2Keys:VMS:Header:IntegratingAgency"] ?? "",
                    bill_code = 0
                };

                // 4. Send Request
                string requestJson = JsonSerializer.Serialize(requestWrapper);
                var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{_ifmsBaseUrl}vmslogin", content);
                if (!response.IsSuccessStatusCode)
                {
                    return (false, null, $"IFMS Server returned error: {response.StatusCode}");
                }

                string responseString = await response.Content.ReadAsStringAsync();
                var wrapper = JsonSerializer.Deserialize<IfmsResponseWrapper>(responseString);

                if (wrapper == null || wrapper.status != 200)
                {
                    return (false, null, wrapper?.msg ?? "IFMS Login Failed");
                }

                // 5. Decrypt Result
                string decryptedResultJson = Decrypt(wrapper.result);
                var loginResponse = JsonSerializer.Deserialize<IfmsLoginResponse>(decryptedResultJson);

                if (loginResponse?.data == null)
                {
                    return (false, null, "Invalid response format from IFMS");
                }

                return (true, loginResponse.data, "Success");
            }
            catch (Exception ex)
            {
                return (false, null, $"IFMS Integration Error: {ex.Message}");
            }
        }

        public string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(_encryptionKey);
                aesAlg.IV = Encoding.UTF8.GetBytes(_encryptionIv);
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }

        public string Decrypt(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(_encryptionKey);
                aesAlg.IV = Encoding.UTF8.GetBytes(_encryptionIv);
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        public string GenerateChecksum(string json)
        {
            var encoding = new UTF8Encoding();
            byte[] keyBytes = encoding.GetBytes(_checksumKey);
            byte[] messageBytes = encoding.GetBytes(json);

            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(messageBytes);
                return BitConverter.ToString(hashValue).Replace("-", "").ToLower();
            }
        }
    }
}

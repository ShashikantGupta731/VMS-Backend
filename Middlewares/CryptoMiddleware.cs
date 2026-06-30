using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System;
using backend.Services;
using System.Text.Json;
using System.Security.Cryptography;

namespace backend.Middlewares
{
    public class CryptoMiddleware
    {
        private readonly RequestDelegate _next;

        public CryptoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IRsaKeyService rsaKeyService)
        {
            var path = context.Request.Path.Value;
            
            // Skip endpoints that shouldn't be encrypted
            if (path.Contains("/api/auth/public-key", StringComparison.OrdinalIgnoreCase) || 
                path.Contains("/swagger", StringComparison.OrdinalIgnoreCase) || 
                path.Contains("/api/upload", StringComparison.OrdinalIgnoreCase) ||
                path.Contains("/api/IfmsIntegration", StringComparison.OrdinalIgnoreCase) ||
                path.Contains("/api/Mobile", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            Console.WriteLine($"\n[CryptoMiddleware] Intercepting request to: {path}");

            // --- DECRYPTION (Incoming Request) ---
            string requestAesKey = null;

            if ((context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put) && context.Request.ContentLength > 0)
            {
                // Read headers
                if (context.Request.Headers.TryGetValue("X-Encrypted-Key", out var encryptedKeyHeader))
                {
                    requestAesKey = rsaKeyService.DecryptAesKey(encryptedKeyHeader.ToString());
                    
                    if (requestAesKey != null && requestAesKey.StartsWith("ERROR:"))
                    {
                        Console.WriteLine($"[CryptoMiddleware] FAILED to decrypt AES key from headers: {requestAesKey}");
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync($"Invalid encryption key. {requestAesKey}");
                        return;
                    }

                    if (string.IsNullOrEmpty(requestAesKey))
                    {
                        Console.WriteLine("[CryptoMiddleware] FAILED to decrypt AES key from headers.");
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Invalid encryption key.");
                        return;
                    }

                    using (StreamReader reader = new StreamReader(context.Request.Body))
                    {
                        string encryptedBody = await reader.ReadToEndAsync();
                        Console.WriteLine($"[CryptoMiddleware] Received Encrypted Payload (Raw): {encryptedBody}");

                        try
                        {
                            string decryptedBody = DecryptWithAes(encryptedBody, requestAesKey);
                            Console.WriteLine($"[CryptoMiddleware] Decrypted Payload: {decryptedBody}");
                            
                            // Replace the request body stream with decrypted content
                            var bytes = Encoding.UTF8.GetBytes(decryptedBody);
                            context.Request.Body = new MemoryStream(bytes);
                            context.Request.ContentLength = bytes.Length;
                            
                            // EXTREMELY IMPORTANT: We must tell the next middleware/controller that the decrypted body is JSON!
                            // Otherwise, it inherits text/plain from the frontend and returns 415 Unsupported Media Type.
                            context.Request.ContentType = "application/json";
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[CryptoMiddleware] FAILED to decrypt payload: {ex.Message}");
                            context.Response.StatusCode = 400;
                            await context.Response.WriteAsync("Decryption failed.");
                            return;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("[CryptoMiddleware] WARNING: No X-Encrypted-Key header found. Proceeding as plain text.");
                }
            }

            // --- ENCRYPTION (Outgoing Response) ---
            var originalBodyStream = context.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                await _next(context);

                if (requestAesKey != null && context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
                {
                    responseBody.Seek(0, SeekOrigin.Begin);
                    using (var streamReader = new StreamReader(responseBody))
                    {
                        string plainResponse = await streamReader.ReadToEndAsync();
                        Console.WriteLine($"[CryptoMiddleware] Original Response Payload: {plainResponse}");

                        if (!string.IsNullOrEmpty(plainResponse))
                        {
                            string encryptedResponse = EncryptWithAes(plainResponse, requestAesKey);
                            Console.WriteLine($"[CryptoMiddleware] Encrypted Response Payload: {encryptedResponse}");

                            var encryptedBytes = Encoding.UTF8.GetBytes(encryptedResponse);
                            
                            // Adjust response properties
                            context.Response.ContentType = "application/json";
                            context.Response.ContentLength = encryptedBytes.Length;

                            context.Response.Body = originalBodyStream;
                            await context.Response.Body.WriteAsync(encryptedBytes, 0, encryptedBytes.Length);
                            return;
                        }
                    }
                }

                // If no encryption needed or if it was empty
                responseBody.Seek(0, SeekOrigin.Begin);
                context.Response.Body = originalBodyStream;
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private string DecryptWithAes(string encryptedDataObjString, string aesKeyBase64)
        {
            var parsed = JsonSerializer.Deserialize<EncryptedPayload>(encryptedDataObjString);
            var cipherBytes = Convert.FromBase64String(parsed.cipher);
            var ivBytes = Convert.FromBase64String(parsed.iv);
            var keyBytes = Convert.FromBase64String(aesKeyBase64);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes;
                aesAlg.IV = ivBytes;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }

        private string EncryptWithAes(string plainText, string aesKeyBase64)
        {
            var keyBytes = Convert.FromBase64String(aesKeyBase64);
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes;
                aesAlg.GenerateIV();
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    
                    var cipherBase64 = Convert.ToBase64String(msEncrypt.ToArray());
                    var ivBase64 = Convert.ToBase64String(aesAlg.IV);

                    var payload = new EncryptedPayload { iv = ivBase64, cipher = cipherBase64 };
                    return JsonSerializer.Serialize(payload);
                }
            }
        }

        private class EncryptedPayload
        {
            public string iv { get; set; }
            public string cipher { get; set; }
        }
    }
}

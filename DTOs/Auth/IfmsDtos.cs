using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Auth
{
    public class IfmsLoginRequest
    {
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
    }

    public class IfmsEncryptedPayload
    {
        public object data { get; set; } = null!;
        public string checksum { get; set; } = string.Empty;
    }

    public class IfmsRequestWrapper
    {
        public string encData { get; set; } = string.Empty;
        public string client_id { get; set; } = string.Empty;
        public string client_secret { get; set; } = string.Empty;
        public string ipaddress { get; set; } = string.Empty;
        public string integration_src { get; set; } = string.Empty;
        public long bill_code { get; set; }
    }

    public class IfmsResponseWrapper
    {
        public string msg { get; set; } = string.Empty;
        public int status { get; set; }
        public string result { get; set; } = string.Empty; // Encrypted result string
    }

    public class IfmsLoginResponseData
    {
        public int status { get; set; }
        public string msg { get; set; } = string.Empty;
        public string ddoCode { get; set; } = string.Empty;
        public string ddoUserName { get; set; } = string.Empty;
        public string clerkName { get; set; } = string.Empty;
    }

    public class IfmsLoginResponse
    {
        public IfmsLoginResponseData data { get; set; } = new();
    }
}

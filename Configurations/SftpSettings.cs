namespace backend.Configurations
{
    public class SftpSettings
    {
        public string HostName { get; set; } = string.Empty;
        public int PortNumber { get; set; } = 22;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string SshHostKeyFingerprint { get; set; } = string.Empty;
    }
}

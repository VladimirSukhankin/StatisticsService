namespace StatisticsService.Core.Settings;

public class DataBaseSettings
{
    public string Host { get; set; } = null!;

    public ushort Port { get; set; }
    
    public string User { get; set; }

    public string Password { get; set; }

    public string Database { get; set; }
    
    public bool Compress { get; set; }

    public bool CheckCompressedHash { get; set; }

    public long SocketTimeout { get; set; }

    public string Compressor { get; set; }
}
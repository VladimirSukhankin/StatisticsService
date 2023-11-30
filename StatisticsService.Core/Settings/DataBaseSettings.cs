namespace StatisticsService.Core.Settings;

public class DataBaseSettings
{
    public string Host { get; set; } = null!;

    public ushort Port { get; set; }
    
    public string User { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Database { get; set; } = null!;

    public bool Compress { get; set; }

    public bool CheckCompressedHash { get; set; }

    public long SocketTimeout { get; set; }

    public string Compressor { get; set; } = null!;
}
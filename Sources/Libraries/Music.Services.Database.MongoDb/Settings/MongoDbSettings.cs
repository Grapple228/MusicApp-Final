using Music.Services.Common;

namespace Music.Services.Database.MongoDb.Settings;

public class MongoDbSettings : ISettings
{
    private readonly string _connectionString;

    public string Host { get; init; } = null!;
    public int Port { get; init; }

    public string ConnectionString
    {
        get =>
            string.IsNullOrWhiteSpace(_connectionString)
                ? $"mongodb://{Host}:{Port}"
                : _connectionString;
        init => _connectionString = value;
    }
}
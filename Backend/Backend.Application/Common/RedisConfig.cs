namespace Backend.Application.Common;

public class RedisConfig
{
    public string ConnectionString { get; set; } = null!;
    public int Database { get; set; }
    public string KeyPrefix { get; set; } = "purefit";
    public int DefaultExpirationMinutes { get; set; } = 120;
    public int ExerciseListTtlHours { get; set; } = 2;
    public int ExerciseDetailTtlHours { get; set; } = 24;
    public int StaticDataTtlHours { get; set; } = 24;
    public int ConnectTimeout { get; set; } = 5000;
    public int CommandTimeout { get; set; } = 5000;
    public int ConnectRetry { get; set; } = 3;
    public bool Ssl { get; set; }
    public string? Password { get; set; }
}

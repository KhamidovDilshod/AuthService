namespace AuthService.Common.RabbitMq;

public class RabbitMqOptions
{
    public string Namespace { get; set; } = string.Empty;
    public int Retries { get; set; }
    public int RetryInterval { get; set; }
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string Host { get; set; } = "localhost";
    public string VirtualHost { get; set; } = "/";
    public int Port { get; set; }
}
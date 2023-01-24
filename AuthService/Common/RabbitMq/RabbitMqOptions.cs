namespace AuthService.Common.RabbitMq;

public class RabbitMqOptions
{
    public string Namespace { get; set; }
    public int Retries { get; set; }
    public int RetryInterval { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public string VirtualHost { get; set; }
}
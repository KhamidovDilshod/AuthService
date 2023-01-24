namespace AuthService.Common.RabbitMq;

public class RabbitMqOptions
{
    public string Namespace { get; set; }
    public int Retries { get; set; }
    public int RetryInterval { get; set; }
}
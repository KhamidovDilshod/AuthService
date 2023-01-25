namespace AuthService.Common.Messages;

public interface IMessage
{
    public string Message { get; set; }
    public Guid UserId { get; }
}
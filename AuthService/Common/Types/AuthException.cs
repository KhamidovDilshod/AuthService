namespace AuthService.Common.Types;

public class AuthException : Exception
{
    public string Code { get; }

    public AuthException()
    {
    }

    public AuthException(string code)
    {
        Code = code;
    }

    public AuthException(string message, params object[] args)
        : this(string.Empty, message, args)
    {
    }

    public AuthException(string code, string message, params object[] args)
        : this(null, code, message, args)
    {
    }

    public AuthException(Exception innerException, string message, params object[] args)
        : this(innerException, string.Empty, message, args)
    {
    }

    public AuthException(Exception innerException, string code, string message, params object[] args)
        : base(string.Format(message, args), innerException)
    {
        Code = code;
    }
}
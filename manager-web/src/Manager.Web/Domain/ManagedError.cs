namespace Manager.Web.Domain;

public class ManagedError
{
    public string Message { get; }

    public ManagedError(string message)
    {
        Message = message;
    }
}
namespace Manager.Web.Domain;

/// <summary>
/// A wrapper for Server errors that encompasses data about a specific error that occured
/// </summary>
public sealed class ManagedError
{
    /// <summary>
    /// The error message from the Server
    /// </summary>
    public string Message { get; }

    public ManagedError(string message)
    {
        Message = message;
    }
}
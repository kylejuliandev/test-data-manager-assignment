namespace Manager.Web.Domain;

/// <summary>
/// A domain wrapper for responses from the Server
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class ManagedResponse<T>
{
    /// <summary>
    /// The item retrieved from the Server, may be null
    /// </summary>
    public T? Item { get; }

    /// <summary>
    /// Errors returned by the server
    /// </summary>
    public ManagedError[] Errors { get; }

    public ManagedResponse(T? item, ManagedError[]? errors = null)
    {
        Item = item;
        Errors = errors ?? Array.Empty<ManagedError>();
    }
}

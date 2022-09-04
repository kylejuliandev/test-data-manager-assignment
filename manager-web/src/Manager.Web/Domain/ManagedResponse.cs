namespace Manager.Web.Domain;

public class ManagedResponse<T>
{
    public T? Item { get; }

    public ManagedError[] Errors { get; }

    public ManagedResponse(T? item, ManagedError[]? errors = null)
    {
        Item = item;
        Errors = errors ?? Array.Empty<ManagedError>();
    }
}

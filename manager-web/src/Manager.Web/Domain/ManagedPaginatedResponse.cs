namespace Manager.Web.Domain;

public class ManagedPaginatedResponse<T>
{
    public T[] Items { get; }

    public bool HasMore { get; }

    public ManagedError[] Errors { get; }

    public ManagedPaginatedResponse(T[]? items = null, bool hasMore = false, ManagedError[]? errors = null)
    {
        Items = items ?? Array.Empty<T>();
        HasMore = hasMore;
        Errors = errors ?? Array.Empty<ManagedError>();
    }
}

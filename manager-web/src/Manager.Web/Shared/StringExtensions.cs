namespace Manager.Web.Shared;

public static class StringExtensions
{
    /// <summary>
    /// Attempt to truncate the string and apply a suffix if the string is longer than a desirable length
    /// </summary>
    /// <param name="value"></param>
    /// <param name="maxLength"></param>
    /// <param name="truncationSuffix"></param>
    /// <returns></returns>
    public static string? Truncate(this string? value, int maxLength, string truncationSuffix = "…")
    {
        return value?.Length > maxLength
            ? value[..maxLength] + truncationSuffix
            : value;
    }
}

using Microsoft.EntityFrameworkCore;

namespace Manager.Web.UnitTests;

internal static class EntityExtensions
{
    public static void Clear<T>(this DbSet<T> dbSet) where T : class
    {
        dbSet.RemoveRange(dbSet);
    }
}
using System.Linq.Expressions;

namespace JobNexus.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplySorting<T>(
               this IQueryable<T> query,
               string sortBy,
               bool isDescending,
               Dictionary<string, Expression<Func<T, object>>> sortMap)
        {
            if (!sortMap.TryGetValue(sortBy, out var expression))
                return query;

            return isDescending
                ? query.OrderByDescending(expression)
                : query.OrderBy(expression);
        }
    }
}

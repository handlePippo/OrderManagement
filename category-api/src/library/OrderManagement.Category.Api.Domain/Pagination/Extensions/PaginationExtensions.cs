using System.Linq.Expressions;

namespace OrderManagement.Category.Api.Domain.Pagination.Extensions
{
    public static class PaginationExtensions
    {
        public static IOrderedQueryable<TSource> ApplyOrder<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, PageOrder order)
            where TSource : class
            where TKey : notnull
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(keySelector);

            return order switch
            {
                PageOrder.Asc => source.OrderBy(keySelector),
                PageOrder.Desc => source.OrderByDescending(keySelector),
                _ => throw new ArgumentOutOfRangeException(nameof(order))
            };
        }

        public static IQueryable<TSource> ApplyPagination<TSource>(this IQueryable<TSource> source, int page, int size)
            where TSource : class
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
            ArgumentOutOfRangeException.ThrowIfLessThan(size, 1);

            return source
                    .Skip((page - 1) * size)
                    .Take(size);
        }
    }
}

namespace OrderManagement.Category.Api.Domain.Pagination
{
    public sealed class ListRequest
    {
        public int Page { get; private init; }
        public int Size { get; private init; }
        public PageOrder Order { get; private init; }

        /// <summary>
        /// Contrusctor for Automapper.
        /// </summary>
        private ListRequest() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="order"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ListRequest(int page, int size, PageOrder order)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
            Page = page;

            ArgumentOutOfRangeException.ThrowIfLessThan(size, 1);
            Size = size;

            if (order == PageOrder.None)
            {
                throw new ArgumentOutOfRangeException(nameof(order));
            }

            Order = order;
        }
    }
}
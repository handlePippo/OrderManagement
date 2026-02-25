namespace OrderManagement.Order.Api.Application.Interfaces
{
    public interface ICurrentUserProvider
    {
        public bool IsAdmin { get; }
        public int GetLoggedUserId();
    }
}
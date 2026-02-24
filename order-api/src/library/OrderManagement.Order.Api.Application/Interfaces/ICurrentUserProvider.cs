namespace OrderManagement.Order.Api.Application.Interfaces
{
    public interface ICurrentUserProvider
    {
        public int GetLoggedUserId();
        public string GetLoggedUsername();
    }
}
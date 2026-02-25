namespace OrderManagement.Gateway.Application.Interfaces
{
    public interface ICurrentUserProvider
    {
        public bool IsAdmin { get; }
        public int GetLoggedUserId();
    }
}
namespace OrderManagement.Provisioner.Api.Application.Interfaces
{
    public interface ICurrentUserProvider
    {
        public bool IsAdmin { get; }
        public int GetLoggedUserId();
        public string GetLoggedUsername();
    }
}
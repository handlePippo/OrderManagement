namespace OrderManagement.Provisioner.Api.Application.Interfaces
{
    public interface ICurrentUserProvider
    {
        public int GetLoggedUserId();
        public string GetLoggedUsername();
    }
}
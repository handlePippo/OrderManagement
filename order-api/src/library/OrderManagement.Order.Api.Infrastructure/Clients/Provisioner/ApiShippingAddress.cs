namespace OrderManagement.Order.Api.Infrastructure.Clients.Provisioner;

public sealed class ApiShippingAddress
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public int UserId { get; set; }
    public string CountryCode { get; set; } = null!;
    public string City { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string Street { get; set; } = null!;
}


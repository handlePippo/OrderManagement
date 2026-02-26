namespace OrderManagement.Provisioner.Api.Infrastructure.Entities;

/// <summary>
/// Address entity.
/// </summary>
public sealed class AddressEntity : EntityBase
{
    /// <summary>
    /// User id.
    /// </summary>
    public int UserId { get; private set; }

    /// <summary>
    /// Country code.
    /// </summary>
    public string CountryCode { get; private set; } = null!;

    /// <summary>
    /// City.
    /// </summary>
    public string City { get; private set; } = null!;

    /// <summary>
    /// Postal code.
    /// </summary>
    public string PostalCode { get; private set; } = null!;

    /// <summary>
    /// Street name.
    /// </summary>
    public string Street { get; private set; } = null!;

    /// <summary>
    /// Constructor for EF / Automapper.
    /// </summary>
    private AddressEntity() { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="countryCode"></param>
    /// <param name="city"></param>
    /// <param name="postalCode"></param>
    /// <param name="street"></param>
    public AddressEntity(int userId,
        string countryCode,
        string city,
        string postalCode,
        string street)
    {
        UserId = userId;
        CountryCode = countryCode;
        City = city;
        PostalCode = postalCode;
        Street = street;
    }
}
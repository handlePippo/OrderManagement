namespace OrderManagement.Provisioner.Api.Domain.Entities;

/// <summary>
/// Address
/// </summary>
public sealed class Address : DomainEntityBase
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
    /// Street
    /// </summary>
    public string Street { get; private set; } = null!;

    /// <summary>
    /// Constructo for Automapper.
    /// </summary>
    private Address() { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    public Address(int id, int userId) : base(id)
    {
        UserId = userId;
    }

    /// <summary>
    /// Sets the user id.
    /// </summary>
    /// <param name="userId"></param>
    public void SetUserId(int userId)
    {
        UserId = userId;
    }

    /// <summary>
    /// Sets the country code.
    /// </summary>
    /// <param name="countryCode"></param>
    public void SetCountryCode(string countryCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(countryCode, nameof(countryCode));

        CountryCode = countryCode.Trim();
    }

    /// <summary>
    /// Sets the city.
    /// </summary>
    /// <param name="city"></param>
    public void SetCity(string city)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(city, nameof(city));

        City = city.Trim();
    }

    /// <summary>
    /// Sets the postal code.
    /// </summary>
    /// <param name="postalCode"></param>
    public void SetPostalCode(string postalCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(postalCode, nameof(postalCode));

        PostalCode = postalCode.Trim();
    }

    /// <summary>
    /// Sets the street.
    /// </summary>
    /// <param name="street"></param>
    public void SetStreet(string street)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(street, nameof(street));

        Street = street.Trim();
    }
}

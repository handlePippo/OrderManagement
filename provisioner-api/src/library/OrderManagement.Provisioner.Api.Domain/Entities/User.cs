using System.Text;

namespace OrderManagement.Provisioner.Api.Domain.Entities;

/// <summary>
/// User
/// </summary>
public sealed class User : DomainEntityBase
{
    /// <summary>
    /// Email.
    /// </summary>
    public string Email { get; private set; } = null!;

    /// <summary>
    /// Password in base64.
    /// </summary>
    public string PasswordHash { get; private set; } = null!;

    /// <summary>
    /// Phone number.
    /// </summary>
    public string PhoneNumber { get; private set; } = null!;

    /// <summary>
    /// First name.
    /// </summary>
    public string FirstName { get; private set; } = null!;

    /// <summary>
    /// Last name.
    /// </summary>
    public string LastName { get; private set; } = null!;

    /// <summary>
    /// Constructor for Automapper.
    /// </summary>
    private User() { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="id"></param>
    public User(int id) : base(id) { }

    /// <summary>
    /// Sets the email.
    /// </summary>
    /// <param name="email"></param>
    public void SetEmail(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email, nameof(email));

        Email = email.Trim();
    }

    /// <summary>
    /// Sets the password.
    /// </summary>
    /// <param name="password"></param>
    public void SetPasswordHash(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));

        PasswordHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
    }

    /// <summary>
    /// Sets the phone number.
    /// </summary>
    /// <param name="phoneNumber"></param>
    public void SetPhoneNumber(string phoneNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));

        PhoneNumber = phoneNumber.Trim();
    }

    /// <summary>
    /// Sets the first name.
    /// </summary>
    /// <param name="firstName"></param>
    public void SetFirstName(string firstName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName, nameof(firstName));

        FirstName = firstName.Trim();
    }

    /// <summary>
    /// Sets the last name.
    /// </summary>
    /// <param name="lastName"></param>
    public void SetLastName(string lastName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName, nameof(lastName));

        LastName = lastName.Trim();
    }
}
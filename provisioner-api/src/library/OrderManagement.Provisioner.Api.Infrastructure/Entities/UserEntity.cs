namespace OrderManagement.Provisioner.Api.Infrastructure.Entities;

/// <summary>
/// User entity.
/// </summary>
public sealed class UserEntity : EntityBase
{
    /// <summary>
    /// Email.
    /// </summary>
    public string Email { get; private set; } = null!;

    /// <summary>
    /// Passowrd hash.
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
    /// Private constructor for EF / Automapper.
    /// </summary>
    private UserEntity() { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="passwordHash"></param>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="phoneNumber"></param>
    public UserEntity(int id,
        string email,
        string passwordHash,
        string firstName,
        string lastName,
        string phoneNumber) : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
    }
}
namespace OrderManagement.Provisioner.Api.Domain.ValueObjects;

/// <summary>
/// Token request domain entity.
/// </summary>
public sealed record TokenRequest
{
    /// <summary>
    /// Email.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// Constructor for Automapper.
    /// </summary>
    private TokenRequest() { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    public TokenRequest(string username, string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username, nameof(username));
        ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));
    }
}

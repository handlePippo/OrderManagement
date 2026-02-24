using System.Security.Claims;

namespace OrderManagement.Provisioner.Api.Persistence.Extensions
{
    /// <summary>
    /// Claims extensions.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Gets the user id.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public static int GetLoggedUserId(this ClaimsPrincipal user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userId, out var id))
            {
                throw new UnauthorizedAccessException("Invalid sub claim");
            }

            return id;
        }

        /// <summary>
        /// Gets the user id.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public static string GetLoggedUsername(this ClaimsPrincipal user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var username = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new UnauthorizedAccessException("Invalid username claim");
            }

            return username;
        }
    }
}

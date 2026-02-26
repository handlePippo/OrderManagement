using OrderManagement.Order.Api.Application.Configuration;
using System.Security.Claims;

namespace OrderManagement.Order.Api.Infrastructure.Extensions
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
        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var userId = user.FindFirst(ClaimTypes.Role)?.Value;

            return userId == UserRoles.Admin;
        }

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
    }
}

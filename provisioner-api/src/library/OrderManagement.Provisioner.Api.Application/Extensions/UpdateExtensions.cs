using OrderManagement.Provisioner.Api.Application.DTOs.Addresses;
using OrderManagement.Provisioner.Api.Application.DTOs.Users;
using OrderManagement.Provisioner.Api.Domain.Entities;

namespace OrderManagement.Provisioner.Api.Application.Extensions
{
    public static class UpdateExtensions
    {
        public static void ApplyPatchFrom(this User user, UpdateUserDto dto)
        {
            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(dto);

            var updateCount = 0;
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                user.SetEmail(dto.Email);
                updateCount++;
            }

            if (!string.IsNullOrWhiteSpace(dto.FirstName))
            {
                user.SetFirstName(dto.FirstName);
                updateCount++;
            }

            if (!string.IsNullOrWhiteSpace(dto.LastName))
            {
                user.SetLastName(dto.LastName);
                updateCount++;
            }

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                user.SetPhoneNumber(dto.PhoneNumber);
                updateCount++;
            }

            if (updateCount > 0)
            {
                user.MarkModified();
            }
        }

        public static void ApplyPatchFrom(this Address address, UpdateAddressDto dto)
        {
            ArgumentNullException.ThrowIfNull(address);
            ArgumentNullException.ThrowIfNull(dto);

            var updateCount = 0;
            if (!string.IsNullOrWhiteSpace(dto.CountryCode))
            {
                address.SetCountryCode(dto.CountryCode);
                updateCount++;
            }

            if (!string.IsNullOrWhiteSpace(dto.City))
            {
                address.SetCity(dto.City);
                updateCount++;
            }

            if (!string.IsNullOrWhiteSpace(dto.PostalCode))
            {
                address.SetPostalCode(dto.PostalCode);
                updateCount++;
            }

            if (!string.IsNullOrWhiteSpace(dto.Street))
            {
                address.SetStreet(dto.Street);
                updateCount++;
            }

            if (updateCount > 0)
            {
                address.MarkModified();
            }
        }
    }
}
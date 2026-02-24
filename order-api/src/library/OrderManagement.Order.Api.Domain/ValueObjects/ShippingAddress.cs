namespace OrderManagement.Order.Api.Domain.ValueObjects
{
    public sealed record ShippingAddress
    {
        public string ShipAddress { get; private set; } = null!;
        public string ShipCity { get; private set; } = null!;
        public string ShipPostalCode { get; private set; } = null!;
        public string ShipCountryCode { get; private set; } = null!;
        public string ShipPhoneNumber { get; private set; } = null!;

        public void SetShipAddress(string shipAddress)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(shipAddress);

            ShipAddress = shipAddress.Trim();
        }

        public void SetShipCity(string shipCity)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(shipCity);

            ShipCity = shipCity.Trim();
        }

        public void SetShipPostalCode(string shipPostalCode)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(shipPostalCode);

            ShipPostalCode = shipPostalCode.Trim();
        }

        public void SetShipCountryCode(string shipCountryCode)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(shipCountryCode);

            ShipCountryCode = shipCountryCode.Trim();
        }

        public void SetShipPhoneNumber(string shipPhoneNumber)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(shipPhoneNumber);

            ShipPhoneNumber = shipPhoneNumber.Trim();
        }
    }
}
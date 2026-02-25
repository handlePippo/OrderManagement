namespace OrderManagement.Gateway.Application.DTOs.Provisioners.Users
{
    public sealed record UserDto : EntityBaseDto
    {
        public required string Email { get; init; } = null!;
        public required string FirstName { get; init; } = null!;
        public required string LastName { get; init; } = null!;
        public required string PhoneNumber { get; init; } = null!;

        public UserDto(int id) : base(id) { }
    }
}
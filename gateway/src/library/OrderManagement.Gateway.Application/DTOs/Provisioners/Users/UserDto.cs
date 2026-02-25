namespace OrderManagement.Gateway.Application.DTOs.Provisioners.Users
{
    public sealed record UserDto : EntityBaseDto
    {
        public required string Email { get; set; } = null!;
        public required string FirstName { get; set; } = null!;
        public required string LastName { get; set; } = null!;
        public required string PhoneNumber { get; set; } = null!;

        public UserDto(int id) : base(id) { }
    }
}
namespace OrderManagement.Category.Api.Application.DTOs
{
    public sealed record CategoryDto
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }
        public DateTime? ModifiedAt { get; private set; }
    }
}
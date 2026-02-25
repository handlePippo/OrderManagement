namespace OrderManagement.Gateway.Application.DTOs.Categories
{
    public sealed record CategoryDto : EntityBaseDto
    {
        public string Name { get; set; } = null!;

        public CategoryDto(int id) : base(id) { }
    }
}
namespace OrderManagement.Order.Api.Application.DTOs
{
    public abstract record EntityBaseDto
    {
        public int Id { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ModifiedAt { get; private set; }

        protected EntityBaseDto(int id)
        {
            Id = id;
        }
    }
}
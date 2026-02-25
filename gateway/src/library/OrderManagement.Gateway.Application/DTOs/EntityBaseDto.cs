namespace OrderManagement.Gateway.Application.DTOs
{
    public abstract record EntityBaseDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        protected EntityBaseDto(int id)
        {
            Id = id;
        }
    }
}
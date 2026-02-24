using AutoMapper;
using OrderManagement.Order.Api.Persistence.Entities;

namespace OrderManagement.Order.Api.Persistence.Configuration.Automapper
{
    public sealed class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            ConfigureOrderMappings();
        }

        private void ConfigureOrderMappings()
        {
            CreateMap<Domain.Entities.Order, OrderEntity>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.Ignore());

            CreateMap<OrderEntity, Domain.Entities.Order>()
                .ForMember(m => m.Items, opt => opt.Ignore());

            CreateMap<OrderItemEntity, Domain.Entities.OrderItem>().ReverseMap();
        }
    }
}

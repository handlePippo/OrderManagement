using AutoMapper;
using OrderManagement.Order.Api.Persistence.Clients.Product;
using OrderManagement.Order.Api.Persistence.Clients.Provisioner;
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
                .ForMember(d => d.CreatedAt, opt => opt.Ignore());

            CreateMap<OrderEntity, Domain.Entities.Order>()
                .ForMember(m => m.Items, opt => opt.Ignore());

            CreateMap<OrderItemEntity, Domain.Entities.OrderItem>().ReverseMap();

            CreateMap<ApiUser, Domain.Entities.User>();
            CreateMap<ApiProduct, Domain.Entities.Product>();
            CreateMap<ApiShippingAddress, Domain.Entities.ShippingAddress>()
                .ForMember(m => m.ShipCity, t => t.MapFrom(s => s.City))
                .ForMember(m => m.ShipPostalCode, t => t.MapFrom(s => s.PostalCode))
                .ForMember(m => m.ShipCountryCode, t => t.MapFrom(s => s.CountryCode))
                .ForMember(m => m.ShipAddress, t => t.MapFrom(s => s.Street));
        }
    }
}

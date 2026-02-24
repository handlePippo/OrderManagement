using AutoMapper;
using OrderManagement.Order.Api.Application.DTOs.Orders;
using OrderManagement.Order.Api.Domain.Entities;

namespace OrderManagement.Order.Api.Application.Configuration.Automapper
{
    public sealed class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            ConfigureOrderMapping();
        }

        private void ConfigureOrderMapping()
        {
            // DTO -> Domain
            CreateMap<OrderDto, Domain.Entities.Order>().ReverseMap();
            CreateMap<OrderItemDto, OrderItem>()
                .ForMember(d => d.ProductId, opt => opt.MapFrom(s => s.ProductInfo.ProductId))
                .ForMember(d => d.Quantity, opt => opt.MapFrom(s => s.ProductInfo.Quantity))
                .ForMember(d => d.ProductName, opt => opt.Ignore())
                .ForMember(d => d.UnitPrice, opt => opt.Ignore())
                .ForMember(d => d.OrderId, opt => opt.Ignore())
                .ForMember(m => m.CreatedAt, opt => opt.Ignore())
                .ForMember(m => m.ModifiedAt, opt => opt.Ignore());

            // Domain -> DTO
            CreateMap<OrderItem, OrderItemDto>()
                .ConstructUsing(s => new OrderItemDto(s.Id, s.OrderId))
                .ForMember(d => d.ProductInfo, opt => opt.MapFrom(s => new OrderItemProductInfoDto
                {
                    ProductId = s.ProductId,
                    Quantity = s.Quantity
                }));

            // Both
            CreateMap<ShippingAddressDto, ShippingAddress>().ReverseMap();
        }
    }
}
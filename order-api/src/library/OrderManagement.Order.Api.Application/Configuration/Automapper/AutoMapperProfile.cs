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
            CreateMap<OrderItemInDto, OrderItem>()
                .ForMember(d => d.ProductId, opt => opt.MapFrom(s => s.ProductInfo.ProductId))
                .ForMember(d => d.Quantity, opt => opt.MapFrom(s => s.ProductInfo.Quantity))
                .ForMember(d => d.ProductName, opt => opt.Ignore())
                .ForMember(d => d.UnitPrice, opt => opt.Ignore())
                .ForMember(d => d.OrderId, opt => opt.Ignore())
                .ForMember(m => m.CreatedAt, opt => opt.Ignore())
                .ForMember(m => m.ModifiedAt, opt => opt.Ignore());

            // Domain -> DTO
            CreateMap<OrderItem, OrderItemInDto>()
                .ConstructUsing(s => new OrderItemInDto(s.Id, s.OrderId))
                .ForMember(d => d.ProductInfo, opt => opt.MapFrom(s => new OrderItemProductInfoDto
                {
                    ProductId = s.ProductId,
                    Quantity = s.Quantity
                }));

            CreateMap<OrderItem, OrderItemOutDto>();

            // Both
            CreateMap<OrderDto, Domain.Entities.Order>().ReverseMap();
            CreateMap<ShippingAddressDto, ShippingAddress>().ReverseMap();
        }
    }
}
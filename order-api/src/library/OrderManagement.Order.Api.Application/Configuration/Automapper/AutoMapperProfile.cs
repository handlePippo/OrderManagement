using AutoMapper;
using OrderManagement.Order.Api.Application.DTOs.Orders;
using OrderManagement.Order.Api.Application.DTOs.Orders.Create;
using OrderManagement.Order.Api.Application.DTOs.Orders.Update;

namespace OrderManagement.Order.Api.Application.Configuration.Automapper
{
    public sealed class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            ConfigureOrderMapping();
            ConfigureCreateOrderMapping();
            ConfigureUpdateOrderMapping();
        }

        private void ConfigureOrderMapping()
        {
            CreateMap<OrderDto, Domain.Entities.Order>().ReverseMap();
            CreateMap<OrderItemDto, Domain.Entities.OrderItem>().ReverseMap();
            CreateMap<DTOs.Orders.OrderItemProductInfoDto, Domain.ValueObjects.OrderItemProductInfo>().ReverseMap();
            CreateMap<ShippingAddressDto, Domain.ValueObjects.ShippingAddress>().ReverseMap();
        }

        private void ConfigureCreateOrderMapping()
        {
            CreateMap<CreateOrderDto, Domain.Entities.Order>()
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.UserId, opt => opt.Ignore())
                .ForMember(m => m.Status, opt => opt.Ignore())
                .ForMember(m => m.Total, opt => opt.Ignore())
                .ForMember(m => m.Subtotal, opt => opt.Ignore())
                .ForMember(m => m.CreatedAt, opt => opt.Ignore())
                .ForMember(m => m.ModifiedAt, opt => opt.Ignore());

            CreateMap<OrderItemDto, Domain.Entities.OrderItem>()
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.OrderId, opt => opt.Ignore())
                .ForMember(m => m.CreatedAt, opt => opt.Ignore())
                .ForMember(m => m.ModifiedAt, opt => opt.Ignore());

            CreateMap<CreateShippingAddressDto, Domain.ValueObjects.ShippingAddress>();
        }

        private void ConfigureUpdateOrderMapping()
        {
            CreateMap<UpdateOrderDto, Domain.Entities.Order>()
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.UserId, opt => opt.Ignore())
                .ForMember(m => m.Status, opt => opt.Ignore())
                .ForMember(m => m.Total, opt => opt.Ignore())
                .ForMember(m => m.Subtotal, opt => opt.Ignore())
                .ForMember(m => m.CreatedAt, opt => opt.Ignore())
                .ForMember(m => m.ModifiedAt, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((_, _, srcMember) => GetOnlyValorizedProperty(srcMember)));

            CreateMap<OrderItemDto, Domain.Entities.OrderItem>()
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.OrderId, opt => opt.Ignore())
                .ForMember(m => m.CreatedAt, opt => opt.Ignore())
                .ForMember(m => m.ModifiedAt, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((_, _, srcMember) => GetOnlyValorizedProperty(srcMember)));

            CreateMap<UpdateShippingAddressDto, Domain.ValueObjects.ShippingAddress>()
                .ForAllMembers(opt => opt.Condition((_, _, srcMember) => GetOnlyValorizedProperty(srcMember)));
        }

        private static bool GetOnlyValorizedProperty(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value is not string str)
            {
                return true;
            }

            return !string.IsNullOrWhiteSpace(str);
        }
    }
}
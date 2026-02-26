using AutoMapper;
using OrderManagement.Product.Api.Application.DTOs;
using OrderManagement.Product.Api.Domain.Entities;

namespace OrderManagement.Product.Api.Application.Configuration.Automapper
{
    public sealed class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            ConfigureProductMapping();
        }

        private void ConfigureProductMapping()
        {
            CreateMap<Domain.Entities.Product, ProductDto>().ReverseMap();

            CreateMap<CreateProductDto, Domain.Entities.Product>()
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.CreatedAt, opt => opt.Ignore())
                .ForMember(m => m.ModifiedAt, opt => opt.Ignore());

            CreateMap<UpdateProductDto, Domain.Entities.Product>()
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.CreatedAt, opt => opt.Ignore())
                .ForMember(m => m.ModifiedAt, opt => opt.Ignore());

            CreateMap<GetProductRangeDto, ProductRange>();
        }
    }
}
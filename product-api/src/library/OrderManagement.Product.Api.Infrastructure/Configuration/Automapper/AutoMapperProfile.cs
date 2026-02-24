using AutoMapper;
using OrderManagement.Product.Api.Infrastructure.Entities;

namespace OrderManagement.Product.Api.Infrastructure.Configuration.Automapper
{
    public sealed class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            ConfigureProductMapping();
        }

        private void ConfigureProductMapping()
        {
            CreateMap<Domain.Entities.Product, ProductEntity>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.Ignore());

            CreateMap<ProductEntity, Domain.Entities.Product>();
        }
    }
}

using AutoMapper;
using OrderManagement.Category.Api.Application.DTOs;

namespace OrderManagement.Category.Api.Application.Configuration.Automapper
{
    public sealed class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            ConfigureCategoryMapping();
        }

        private void ConfigureCategoryMapping()
        {
            CreateMap<Domain.Entities.Category, CategoryDto>().ReverseMap();

            CreateMap<CreateCategoryDto, Domain.Entities.Category>()
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.CreatedAt, opt => opt.Ignore())
                .ForMember(m => m.ModifiedAt, opt => opt.Ignore());

            CreateMap<UpdateCategoryDto, Domain.Entities.Category>()
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.CreatedAt, opt => opt.Ignore())
                .ForMember(m => m.ModifiedAt, opt => opt.Ignore());
        }
    }
}